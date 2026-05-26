using Microsoft.Data.Sqlite;
using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public sealed class AuthenticationService : SqliteServiceBase, IAuthenticationService
{
    public AuthenticationService(ILocalDatabaseService localDatabaseService)
        : base(localDatabaseService)
    {
    }

    public async Task<IReadOnlyList<DemoAccount>> GetDemoAccountsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        const string sql = """
            SELECT
                u.id,
                u.firstName,
                u.lastName,
                u.email,
                u.passwordHash,
                r.title AS roleTitle
            FROM users u
            INNER JOIN roles r ON r.id = u.idRole
            ORDER BY u.idRole, u.id;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;

        List<DemoAccount> accounts = [];
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            accounts.Add(new DemoAccount
            {
                UserId = reader.GetInt32(reader.GetOrdinal("id")),
                DisplayName = $"{reader.GetString(reader.GetOrdinal("firstName"))} {reader.GetString(reader.GetOrdinal("lastName"))}",
                RoleTitle = reader.GetString(reader.GetOrdinal("roleTitle")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Password = reader.GetString(reader.GetOrdinal("passwordHash"))
            });
        }

        return accounts;
    }

    public async Task<UserSession?> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        const string sql = """
            SELECT
                u.id,
                u.firstName,
                u.lastName,
                u.email,
                u.phone,
                r.title AS roleTitle
            FROM users u
            INNER JOIN roles r ON r.id = u.idRole
            WHERE lower(u.email) = lower($email)
              AND u.passwordHash = $password
            LIMIT 1;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$email", email.Trim());
        command.Parameters.AddWithValue("$password", password);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserSession
        {
            UserId = reader.GetInt32(reader.GetOrdinal("id")),
            FirstName = reader.GetString(reader.GetOrdinal("firstName")),
            LastName = reader.GetString(reader.GetOrdinal("lastName")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Phone = reader.GetString(reader.GetOrdinal("phone")),
            RoleTitle = reader.GetString(reader.GetOrdinal("roleTitle"))
        };
    }

    public async Task<UserProfile?> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(readOnly: true, cancellationToken);

        const string sql = """
            SELECT
                u.id,
                u.firstName,
                u.lastName,
                u.email,
                u.phone,
                u.dateCreated,
                r.title AS roleTitle,
                COALESCE((
                    SELECT COUNT(*)
                    FROM rentOrders ro
                    WHERE ro.idUser = u.id
                ), 0) AS totalOrders,
                COALESCE((
                    SELECT COUNT(*)
                    FROM rentOrders ro
                    INNER JOIN orderStatuses os ON os.id = ro.idStatus
                    WHERE ro.idUser = u.id
                      AND os.title IN ('Создан', 'Подтвержден', 'В аренде')
                ), 0) AS activeOrders,
                COALESCE((
                    SELECT COUNT(*)
                    FROM rentOrders ro
                    INNER JOIN orderStatuses os ON os.id = ro.idStatus
                    WHERE ro.idUser = u.id
                      AND os.title = 'Завершен'
                ), 0) AS completedOrders,
                COALESCE((
                    SELECT SUM(p.amount)
                    FROM payments p
                    INNER JOIN paymentStatuses ps ON ps.id = p.idStatus
                    INNER JOIN rentOrders ro ON ro.id = p.idOrder
                    WHERE ro.idUser = u.id
                      AND ps.title = 'Оплачено'
                ), 0) AS totalPaidAmount,
                COALESCE((
                    SELECT SUM(p.amount)
                    FROM payments p
                    INNER JOIN paymentStatuses ps ON ps.id = p.idStatus
                    INNER JOIN rentOrders ro ON ro.id = p.idOrder
                    WHERE ro.idUser = u.id
                      AND ps.title = 'Ожидает оплаты'
                ), 0) AS outstandingAmount
            FROM users u
            INNER JOIN roles r ON r.id = u.idRole
            WHERE u.id = $userId
            LIMIT 1;
            """;

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$userId", userId);

        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserProfile
        {
            UserId = reader.GetInt32(reader.GetOrdinal("id")),
            FirstName = reader.GetString(reader.GetOrdinal("firstName")),
            LastName = reader.GetString(reader.GetOrdinal("lastName")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Phone = reader.GetString(reader.GetOrdinal("phone")),
            RoleTitle = reader.GetString(reader.GetOrdinal("roleTitle")),
            DateCreated = ReadDateTime(reader, "dateCreated"),
            TotalOrders = reader.GetInt32(reader.GetOrdinal("totalOrders")),
            ActiveOrders = reader.GetInt32(reader.GetOrdinal("activeOrders")),
            CompletedOrders = reader.GetInt32(reader.GetOrdinal("completedOrders")),
            TotalPaidAmount = reader.GetInt32(reader.GetOrdinal("totalPaidAmount")),
            OutstandingAmount = reader.GetInt32(reader.GetOrdinal("outstandingAmount"))
        };
    }
}
