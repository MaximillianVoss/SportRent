using Microsoft.Data.Sqlite;

namespace SportRent.Mobile.Services;

/// <summary>
/// Общая база для сервисов, которые работают с локальной SQLite-базой.
/// </summary>
public abstract class SqliteServiceBase
{
    private readonly ILocalDatabaseService _localDatabaseService;

    protected SqliteServiceBase(ILocalDatabaseService localDatabaseService)
    {
        _localDatabaseService = localDatabaseService;
    }

    /// <summary>
    /// Открывает подключение к рабочей копии базы в режиме чтения или записи.
    /// </summary>
    protected async Task<SqliteConnection> OpenConnectionAsync(bool readOnly, CancellationToken cancellationToken)
    {
        string databasePath = await _localDatabaseService.GetWorkingCopyPathAsync(cancellationToken);
        SqliteConnectionStringBuilder builder = new()
        {
            DataSource = databasePath,
            Mode = readOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite,
            Pooling = false
        };

        SqliteConnection connection = new(builder.ToString());
        await connection.OpenAsync(cancellationToken);

        // Включаем внешние ключи для каждого подключения: SQLite не включает их глобально по умолчанию.
        await using SqliteCommand pragmaCommand = connection.CreateCommand();
        pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
        await pragmaCommand.ExecuteNonQueryAsync(cancellationToken);

        return connection;
    }

    /// <summary>
    /// Читает дату из SQLite-строки в едином формате, используемом seed-данными и заказами.
    /// </summary>
    protected static DateTime ReadDateTime(SqliteDataReader reader, string columnName)
    {
        return DateTime.Parse(reader.GetString(reader.GetOrdinal(columnName)));
    }
}
