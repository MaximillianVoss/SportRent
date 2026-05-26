using Microsoft.Maui.Storage;

namespace SportRent.Mobile.Services;

public sealed class LocalDatabaseService : ILocalDatabaseService
{
    private const string PackagedDatabasePath = "Database/sportRent.db";
    private const string DatabaseVersionPreferenceKey = "sport_rent_database_version";
    private const string CurrentDatabaseVersion = "2026.03.15.assets.1";

    private readonly SemaphoreSlim _syncLock = new(1, 1);
    private string? _workingCopyPath;

    public async Task<string> GetWorkingCopyPathAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_workingCopyPath) && File.Exists(_workingCopyPath))
        {
            return _workingCopyPath;
        }

        await _syncLock.WaitAsync(cancellationToken);

        try
        {
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
