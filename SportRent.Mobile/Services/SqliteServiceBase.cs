using Microsoft.Data.Sqlite;

namespace SportRent.Mobile.Services;

public abstract class SqliteServiceBase
{
    private readonly ILocalDatabaseService _localDatabaseService;

    protected SqliteServiceBase(ILocalDatabaseService localDatabaseService)
    {
        _localDatabaseService = localDatabaseService;
    }

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

        await using SqliteCommand pragmaCommand = connection.CreateCommand();
        pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
        await pragmaCommand.ExecuteNonQueryAsync(cancellationToken);

        return connection;
    }

    protected static DateTime ReadDateTime(SqliteDataReader reader, string columnName)
    {
        return DateTime.Parse(reader.GetString(reader.GetOrdinal(columnName)));
    }
}
