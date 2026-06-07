using Microsoft.Maui.Storage;

namespace SportRent.Mobile.Services;

/// <summary>
/// Готовит рабочую копию SQLite-базы из ресурсов приложения.
/// </summary>
public sealed class LocalDatabaseService : ILocalDatabaseService
{
    private const string PackagedDatabasePath = "Database/sportRent.db";
    private const string DatabaseVersionPreferenceKey = "sport_rent_database_version";
    private const string CurrentDatabaseVersion = "2026.03.15.assets.1";

    private readonly SemaphoreSlim _syncLock = new(1, 1);
    private string? _workingCopyPath;

    /// <inheritdoc />
    public async Task<string> GetWorkingCopyPathAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_workingCopyPath) && File.Exists(_workingCopyPath))
        {
            return _workingCopyPath;
        }

        await _syncLock.WaitAsync(cancellationToken);

        try
        {
            // Повторная проверка нужна после входа в синхронизированную секцию:
            // другая операция могла уже подготовить файл базы.
            if (!string.IsNullOrWhiteSpace(_workingCopyPath) && File.Exists(_workingCopyPath))
            {
                return _workingCopyPath;
            }

            string databaseDirectory = Path.Combine(FileSystem.AppDataDirectory, "Database");
            Directory.CreateDirectory(databaseDirectory);

            string workingCopyPath = Path.Combine(databaseDirectory, "sportRent.db");
            string installedVersion = Preferences.Default.Get(DatabaseVersionPreferenceKey, string.Empty);

            if (!File.Exists(workingCopyPath) || !string.Equals(installedVersion, CurrentDatabaseVersion, StringComparison.Ordinal))
            {
                // База из пакета копируется в AppData, потому что SQLite-файл из ресурсов нельзя изменять напрямую.
                await using Stream packagedDatabase = await FileSystem.OpenAppPackageFileAsync(PackagedDatabasePath);
                await using FileStream output = File.Create(workingCopyPath);
                await packagedDatabase.CopyToAsync(output, cancellationToken);
                Preferences.Default.Set(DatabaseVersionPreferenceKey, CurrentDatabaseVersion);
            }

            _workingCopyPath = workingCopyPath;
            return workingCopyPath;
        }
        finally
        {
            _syncLock.Release();
        }
    }
}
