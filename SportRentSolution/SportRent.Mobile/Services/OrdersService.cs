using Microsoft.Data.Sqlite;
using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

/// <summary>
/// Управляет заказами аренды: созданием, оплатой, отменой и чтением истории.
/// </summary>
public sealed class OrdersService : SqliteServiceBase, IOrdersService
{
    public OrdersService(ILocalDatabaseService localDatabaseService)
        : base(localDatabaseService)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserOrder>> GetOrdersAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        // История собирается одним агрегирующим запросом, чтобы экран заказов получил все подписи и суммы сразу.
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

    /// <inheritdoc />
    public async Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: false, cancellationToken);
        await using SqliteTransaction transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        // Перед созданием заказа фиксируем пункт выдачи и доступный остаток выбранной складской позиции.
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

        // Справочные идентификаторы берутся по названиям, чтобы код не зависел от конкретных числовых id seed-данных.
        int orderStatusId = await GetLookupIdAsync(connection, transaction, "orderStatuses", "Создан", cancellationToken);
        int paymentMethodId = await GetLookupIdAsync(connection, transaction, "paymentMethods", "Онлайн", cancellationToken);
        int paymentStatusId = await GetLookupIdAsync(connection, transaction, "paymentStatuses", "Ожидает оплаты", cancellationToken);

        // checked защищает расчет суммы от переполнения при некорректных входных данных.
        int rentalAmount = checked(request.PricePerUnit * request.PeriodCount * request.Quantity);
        int depositAmount = checked(request.DepositPerUnit * request.Quantity);
        DateTime endAt = request.StartAt.AddHours(request.UnitHours * request.PeriodCount);
        string description = string.IsNullOrWhiteSpace(request.Description)
            ? $"Заказ на {request.EquipmentTitle}"
            : request.Description.Trim();

        // Резервируем инвентарь до вставки заказа. Условие availableQuantity >= quantity закрывает гонку между экранами.
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

        // Сам заказ хранит период аренды, итоговую стоимость и связанные пункты выдачи/возврата.
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

        // Строка orderItems связывает заказ с конкретным остатком в пункте проката.
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

        // Мок-платеж создается сразу, чтобы пользователь видел заказ в истории и мог нажать "Оплатить".
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

    /// <inheritdoc />
    public async Task PayOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: false, cancellationToken);
        await using SqliteTransaction transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        // Все изменения оплаты выполняются в одной транзакции, чтобы платеж и статус заказа не расходились.
        int pendingPaymentStatusId = await GetLookupIdAsync(connection, transaction, "paymentStatuses", "Ожидает оплаты", cancellationToken);
        int paidPaymentStatusId = await GetLookupIdAsync(connection, transaction, "paymentStatuses", "Оплачено", cancellationToken);
        int onlinePaymentMethodId = await GetLookupIdAsync(connection, transaction, "paymentMethods", "Онлайн", cancellationToken);
        int createdOrderStatusId = await GetLookupIdAsync(connection, transaction, "orderStatuses", "Создан", cancellationToken);
        int confirmedOrderStatusId = await GetLookupIdAsync(connection, transaction, "orderStatuses", "Подтвержден", cancellationToken);

        // Проверяем принадлежность заказа пользователю до изменения платежа.
        SqliteCommand orderCommand = connection.CreateCommand();
        orderCommand.Transaction = transaction;
        orderCommand.CommandText = """
            SELECT
                idStatus,
                amount + depositAmount AS paymentAmount
            FROM rentOrders
            WHERE id = $orderId
              AND idUser = $userId
            LIMIT 1;
            """;
        orderCommand.Parameters.AddWithValue("$orderId", orderId);
        orderCommand.Parameters.AddWithValue("$userId", userId);

        int orderStatusId;
        int paymentAmount;

        await using (SqliteDataReader orderReader = await orderCommand.ExecuteReaderAsync(cancellationToken))
        {
            if (!await orderReader.ReadAsync(cancellationToken))
            {
                throw new InvalidOperationException("Заказ не найден или принадлежит другому пользователю.");
            }

            orderStatusId = orderReader.GetInt32(orderReader.GetOrdinal("idStatus"));
            paymentAmount = orderReader.GetInt32(orderReader.GetOrdinal("paymentAmount"));
        }

        SqliteCommand paymentLookupCommand = connection.CreateCommand();
        paymentLookupCommand.Transaction = transaction;
        paymentLookupCommand.CommandText = """
            SELECT
                id,
                idStatus
            FROM payments
            WHERE idOrder = $orderId
            ORDER BY id DESC
            LIMIT 1;
            """;
        paymentLookupCommand.Parameters.AddWithValue("$orderId", orderId);

        int? paymentId = null;
        int? paymentStatusId = null;

        await using (SqliteDataReader paymentReader = await paymentLookupCommand.ExecuteReaderAsync(cancellationToken))
        {
            if (await paymentReader.ReadAsync(cancellationToken))
            {
                paymentId = paymentReader.GetInt32(paymentReader.GetOrdinal("id"));
                paymentStatusId = paymentReader.GetInt32(paymentReader.GetOrdinal("idStatus"));
            }
        }

        if (paymentStatusId == paidPaymentStatusId)
        {
            throw new InvalidOperationException("Заказ уже оплачен.");
        }

        // Если платеж уже создан при оформлении заказа, меняем только его статус и способ оплаты.
        if (paymentId.HasValue)
        {
            if (paymentStatusId != pendingPaymentStatusId)
            {
                throw new InvalidOperationException("Текущий статус платежа не позволяет выполнить оплату.");
            }

            SqliteCommand updatePaymentCommand = connection.CreateCommand();
            updatePaymentCommand.Transaction = transaction;
            updatePaymentCommand.CommandText = """
                UPDATE payments
                SET idStatus = $paidStatusId,
                    idPaymentMethod = $paymentMethodId
                WHERE id = $paymentId;
                """;
            updatePaymentCommand.Parameters.AddWithValue("$paidStatusId", paidPaymentStatusId);
            updatePaymentCommand.Parameters.AddWithValue("$paymentMethodId", onlinePaymentMethodId);
            updatePaymentCommand.Parameters.AddWithValue("$paymentId", paymentId.Value);
            await updatePaymentCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        else
        {
            // Запасной путь оставлен для заказов без платежа, если такие появятся после миграций или ручного ввода.
            SqliteCommand createPaymentCommand = connection.CreateCommand();
            createPaymentCommand.Transaction = transaction;
            createPaymentCommand.CommandText = """
                INSERT INTO payments (
                    idOrder,
                    idPaymentMethod,
                    idStatus,
                    amount
                )
                VALUES (
                    $orderId,
                    $paymentMethodId,
                    $paidStatusId,
                    $amount
                );
                """;
            createPaymentCommand.Parameters.AddWithValue("$orderId", orderId);
            createPaymentCommand.Parameters.AddWithValue("$paymentMethodId", onlinePaymentMethodId);
            createPaymentCommand.Parameters.AddWithValue("$paidStatusId", paidPaymentStatusId);
            createPaymentCommand.Parameters.AddWithValue("$amount", paymentAmount);
            await createPaymentCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        // После оплаты созданный заказ становится подтвержденным.
        if (orderStatusId == createdOrderStatusId)
        {
            SqliteCommand updateOrderCommand = connection.CreateCommand();
            updateOrderCommand.Transaction = transaction;
            updateOrderCommand.CommandText = """
                UPDATE rentOrders
                SET idStatus = $confirmedStatusId
                WHERE id = $orderId;
                """;
            updateOrderCommand.Parameters.AddWithValue("$confirmedStatusId", confirmedOrderStatusId);
            updateOrderCommand.Parameters.AddWithValue("$orderId", orderId);
            await updateOrderCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CancelOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: false, cancellationToken);
        await using SqliteTransaction transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        // Отменять можно только созданный заказ, который еще ожидает тестовой оплаты.
        int pendingPaymentStatusId = await GetLookupIdAsync(connection, transaction, "paymentStatuses", "Ожидает оплаты", cancellationToken);
        int createdOrderStatusId = await GetLookupIdAsync(connection, transaction, "orderStatuses", "Создан", cancellationToken);
        int canceledOrderStatusId = await GetLookupIdAsync(connection, transaction, "orderStatuses", "Отменен", cancellationToken);

        SqliteCommand orderCommand = connection.CreateCommand();
        orderCommand.Transaction = transaction;
        orderCommand.CommandText = """
            SELECT idStatus
            FROM rentOrders
            WHERE id = $orderId
              AND idUser = $userId
            LIMIT 1;
            """;
        orderCommand.Parameters.AddWithValue("$orderId", orderId);
        orderCommand.Parameters.AddWithValue("$userId", userId);

        object? orderStatusRaw = await orderCommand.ExecuteScalarAsync(cancellationToken);
        if (orderStatusRaw is null)
        {
            throw new InvalidOperationException("Заказ не найден или принадлежит другому пользователю.");
        }

        int orderStatusId = Convert.ToInt32(orderStatusRaw);
        if (orderStatusId != createdOrderStatusId)
        {
            throw new InvalidOperationException("Можно отменить только созданный неоплаченный заказ.");
        }

        SqliteCommand paymentCommand = connection.CreateCommand();
        paymentCommand.Transaction = transaction;
        paymentCommand.CommandText = """
            SELECT idStatus
            FROM payments
            WHERE idOrder = $orderId
            ORDER BY id DESC
            LIMIT 1;
            """;
        paymentCommand.Parameters.AddWithValue("$orderId", orderId);

        object? paymentStatusRaw = await paymentCommand.ExecuteScalarAsync(cancellationToken);
        if (paymentStatusRaw is null || Convert.ToInt32(paymentStatusRaw) != pendingPaymentStatusId)
        {
            throw new InvalidOperationException("Заказ не ожидает оплаты и не может быть отменен.");
        }

        // Возвращаем в каталог ровно то количество, которое было зарезервировано позициями заказа.
        SqliteCommand restoreStockCommand = connection.CreateCommand();
        restoreStockCommand.Transaction = transaction;
        restoreStockCommand.CommandText = """
            UPDATE rentalPointEquipment
            SET availableQuantity = availableQuantity + (
                SELECT COALESCE(SUM(oi.quantity), 0)
                FROM orderItems oi
                WHERE oi.idOrder = $orderId
                  AND oi.idRentalPointEquipment = rentalPointEquipment.id
            )
            WHERE id IN (
                SELECT idRentalPointEquipment
                FROM orderItems
                WHERE idOrder = $orderId
            );
            """;
        restoreStockCommand.Parameters.AddWithValue("$orderId", orderId);

        int restoredRows = await restoreStockCommand.ExecuteNonQueryAsync(cancellationToken);
        if (restoredRows == 0)
        {
            throw new InvalidOperationException("Не найдены позиции заказа для возврата остатка.");
        }

        SqliteCommand updateOrderCommand = connection.CreateCommand();
        updateOrderCommand.Transaction = transaction;
        updateOrderCommand.CommandText = """
            UPDATE rentOrders
            SET idStatus = $canceledStatusId
            WHERE id = $orderId
              AND idUser = $userId;
            """;
        updateOrderCommand.Parameters.AddWithValue("$canceledStatusId", canceledOrderStatusId);
        updateOrderCommand.Parameters.AddWithValue("$orderId", orderId);
        updateOrderCommand.Parameters.AddWithValue("$userId", userId);

        await updateOrderCommand.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Находит идентификатор справочного значения по его названию.
    /// </summary>
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
