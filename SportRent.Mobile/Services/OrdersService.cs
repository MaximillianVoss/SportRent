using Microsoft.Data.Sqlite;
using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public sealed class OrdersService : SqliteServiceBase, IOrdersService
{
    public OrdersService(ILocalDatabaseService localDatabaseService)
        : base(localDatabaseService)
    {
    }

    public async Task<IReadOnlyList<UserOrder>> GetOrdersAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        const string sql = """
            SELECT
                ro.id,
                os.title AS statusTitle,
                rpIssue.name AS issuePointName,
                rpReturn.name AS returnPointName,
                ro.description,
                ro.dateCreated,
                ro.dateStart,
                ro.dateEnd,
                ro.amount,
                ro.depositAmount,
                COALESCE(SUM(p.amount), ro.amount + ro.depositAmount) AS totalPaymentAmount,
                MAX(pm.title) AS paymentMethodTitle,
                MAX(ps.title) AS paymentStatusTitle,
                COALESCE(GROUP_CONCAT(DISTINCT e.title), 'Инвентарь не указан') AS equipmentSummary
            FROM rentOrders ro
            INNER JOIN orderStatuses os ON os.id = ro.idStatus
            INNER JOIN rentalPoints rpIssue ON rpIssue.id = ro.idRentalPointIssue
            LEFT JOIN rentalPoints rpReturn ON rpReturn.id = ro.idRentalPointReturn
            LEFT JOIN orderItems oi ON oi.idOrder = ro.id
            LEFT JOIN rentalPointEquipment rpe ON rpe.id = oi.idRentalPointEquipment
            LEFT JOIN equipment e ON e.id = rpe.idEquipment
            LEFT JOIN payments p ON p.idOrder = ro.id
            LEFT JOIN paymentMethods pm ON pm.id = p.idPaymentMethod
            LEFT JOIN paymentStatuses ps ON ps.id = p.idStatus
            WHERE ro.idUser = $userId
            GROUP BY
                ro.id,
                os.title,
                rpIssue.name,
                rpReturn.name,
                ro.description,
                ro.dateCreated,
                ro.dateStart,
                ro.dateEnd,
                ro.amount,
                ro.depositAmount
            ORDER BY ro.dateCreated DESC, ro.id DESC;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$userId", userId);

        List<UserOrder> orders = [];
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            orders.Add(new UserOrder
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                StatusTitle = reader.GetString(reader.GetOrdinal("statusTitle")),
                IssuePointName = reader.GetString(reader.GetOrdinal("issuePointName")),
                ReturnPointName = reader.IsDBNull(reader.GetOrdinal("returnPointName")) ? null : reader.GetString(reader.GetOrdinal("returnPointName")),
                EquipmentSummary = reader.GetString(reader.GetOrdinal("equipmentSummary")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                DateCreated = ReadDateTime(reader, "dateCreated"),
                DateStart = ReadDateTime(reader, "dateStart"),
                DateEnd = ReadDateTime(reader, "dateEnd"),
                Amount = reader.GetInt32(reader.GetOrdinal("amount")),
                DepositAmount = reader.GetInt32(reader.GetOrdinal("depositAmount")),
                TotalPaymentAmount = reader.GetInt32(reader.GetOrdinal("totalPaymentAmount")),
                PaymentMethodTitle = reader.IsDBNull(reader.GetOrdinal("paymentMethodTitle")) ? null : reader.GetString(reader.GetOrdinal("paymentMethodTitle")),
                PaymentStatusTitle = reader.IsDBNull(reader.GetOrdinal("paymentStatusTitle")) ? null : reader.GetString(reader.GetOrdinal("paymentStatusTitle"))
            });
        }

        return orders;
    }

    public async Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: false, cancellationToken);
        await using SqliteTransaction transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        SqliteCommand stockCommand = connection.CreateCommand();
        stockCommand.Transaction = transaction;
        stockCommand.CommandText = """
            SELECT
                rpe.idRentalPoint,
                rpe.availableQuantity
            FROM rentalPointEquipment rpe
            WHERE rpe.id = $rentalPointEquipmentId
            LIMIT 1;
            """;
        stockCommand.Parameters.AddWithValue("$rentalPointEquipmentId", request.RentalPointEquipmentId);

        int issuePointId;
        int availableQuantity;

        await using (SqliteDataReader stockReader = await stockCommand.ExecuteReaderAsync(cancellationToken))
        {
            if (!await stockReader.ReadAsync(cancellationToken))
            {
                throw new InvalidOperationException("Выбранный остаток инвентаря не найден.");
            }

            issuePointId = stockReader.GetInt32(stockReader.GetOrdinal("idRentalPoint"));
            availableQuantity = stockReader.GetInt32(stockReader.GetOrdinal("availableQuantity"));
        }

        if (availableQuantity < request.Quantity)
        {
            throw new InvalidOperationException("Недостаточно свободного инвентаря для оформления заказа.");
        }

        int orderStatusId = await GetLookupIdAsync(connection, transaction, "orderStatuses", "Создан", cancellationToken);
        int paymentMethodId = await GetLookupIdAsync(connection, transaction, "paymentMethods", "Онлайн", cancellationToken);
        int paymentStatusId = await GetLookupIdAsync(connection, transaction, "paymentStatuses", "Ожидает оплаты", cancellationToken);

        int rentalAmount = checked(request.PricePerUnit * request.PeriodCount * request.Quantity);
        int depositAmount = checked(request.DepositPerUnit * request.Quantity);
        DateTime endAt = request.StartAt.AddHours(request.UnitHours * request.PeriodCount);
        string description = string.IsNullOrWhiteSpace(request.Description)
            ? $"Заказ на {request.EquipmentTitle}"
            : request.Description.Trim();

        SqliteCommand updateStockCommand = connection.CreateCommand();
        updateStockCommand.Transaction = transaction;
        updateStockCommand.CommandText = """
            UPDATE rentalPointEquipment
            SET availableQuantity = availableQuantity - $quantity
            WHERE id = $rentalPointEquipmentId
              AND availableQuantity >= $quantity;
            """;
        updateStockCommand.Parameters.AddWithValue("$quantity", request.Quantity);
        updateStockCommand.Parameters.AddWithValue("$rentalPointEquipmentId", request.RentalPointEquipmentId);

        int updatedRows = await updateStockCommand.ExecuteNonQueryAsync(cancellationToken);
        if (updatedRows == 0)
        {
            throw new InvalidOperationException("Свободный остаток изменился. Обновите карточку и попробуйте снова.");
        }

        SqliteCommand orderCommand = connection.CreateCommand();
        orderCommand.Transaction = transaction;
        orderCommand.CommandText = """
            INSERT INTO rentOrders (
                idUser,
                idStatus,
                idRentalPointIssue,
                idRentalPointReturn,
                dateStart,
                dateEnd,
                amount,
                depositAmount,
                description
            )
            VALUES (
                $userId,
                $statusId,
                $issuePointId,
                $returnPointId,
                $dateStart,
                $dateEnd,
                $amount,
                $depositAmount,
                $description
            );

            SELECT last_insert_rowid();
            """;
        orderCommand.Parameters.AddWithValue("$userId", request.UserId);
        orderCommand.Parameters.AddWithValue("$statusId", orderStatusId);
        orderCommand.Parameters.AddWithValue("$issuePointId", issuePointId);
        orderCommand.Parameters.AddWithValue("$returnPointId", issuePointId);
        orderCommand.Parameters.AddWithValue("$dateStart", request.StartAt.ToString("yyyy-MM-dd HH:mm:ss"));
        orderCommand.Parameters.AddWithValue("$dateEnd", endAt.ToString("yyyy-MM-dd HH:mm:ss"));
        orderCommand.Parameters.AddWithValue("$amount", rentalAmount);
        orderCommand.Parameters.AddWithValue("$depositAmount", depositAmount);
        orderCommand.Parameters.AddWithValue("$description", description);

        long orderIdRaw = (long)(await orderCommand.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException("Не удалось создать заказ."));
        int orderId = checked((int)orderIdRaw);

        SqliteCommand orderItemCommand = connection.CreateCommand();
        orderItemCommand.Transaction = transaction;
        orderItemCommand.CommandText = """
            INSERT INTO orderItems (
                idOrder,
                idRentalPointEquipment,
                quantity,
                pricePerUnit,
                amount
            )
            VALUES (
                $orderId,
                $rentalPointEquipmentId,
                $quantity,
                $pricePerUnit,
                $amount
            );
            """;
        orderItemCommand.Parameters.AddWithValue("$orderId", orderId);
        orderItemCommand.Parameters.AddWithValue("$rentalPointEquipmentId", request.RentalPointEquipmentId);
        orderItemCommand.Parameters.AddWithValue("$quantity", request.Quantity);
        orderItemCommand.Parameters.AddWithValue("$pricePerUnit", request.PricePerUnit * request.PeriodCount);
        orderItemCommand.Parameters.AddWithValue("$amount", rentalAmount);
        await orderItemCommand.ExecuteNonQueryAsync(cancellationToken);

        SqliteCommand paymentCommand = connection.CreateCommand();
        paymentCommand.Transaction = transaction;
        paymentCommand.CommandText = """
            INSERT INTO payments (
                idOrder,
                idPaymentMethod,
                idStatus,
                amount
            )
            VALUES (
                $orderId,
                $paymentMethodId,
                $paymentStatusId,
                $amount
            );
            """;
        paymentCommand.Parameters.AddWithValue("$orderId", orderId);
        paymentCommand.Parameters.AddWithValue("$paymentMethodId", paymentMethodId);
        paymentCommand.Parameters.AddWithValue("$paymentStatusId", paymentStatusId);
        paymentCommand.Parameters.AddWithValue("$amount", rentalAmount + depositAmount);
        await paymentCommand.ExecuteNonQueryAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return orderId;
    }

    private static async Task<int> GetLookupIdAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string tableName,
        string title,
        CancellationToken cancellationToken)
    {
        SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT id FROM {tableName} WHERE title = $title LIMIT 1;";
        command.Parameters.AddWithValue("$title", title);

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        if (result is null)
        {
            throw new InvalidOperationException($"Не найдено значение справочника '{title}' в таблице {tableName}.");
        }

        return Convert.ToInt32(result);
    }
}
