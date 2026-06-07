namespace SportRent.Mobile.Services;

/// <summary>
/// Предоставляет путь к рабочей копии локальной SQLite-базы приложения.
/// </summary>
public interface ILocalDatabaseService
{
    /// <summary>
    /// Подготавливает базу из ресурсов приложения и возвращает путь к файлу для чтения или записи.
    /// </summary>
    Task<string> GetWorkingCopyPathAsync(CancellationToken cancellationToken = default);
}
