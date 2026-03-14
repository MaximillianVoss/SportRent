using Microsoft.Maui.Storage;

namespace SportRent.Mobile.Services;

public sealed class LocalDatabaseService : ILocalDatabaseService
{
    private const string PackagedDatabasePath = "Database/sportRent.db";

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

            if (!File.Exists(workingCopyPath))
            {
                await using Stream packagedDatabase = await FileSystem.OpenAppPackageFileAsync(PackagedDatabasePath);
                await using FileStream output = File.Create(workingCopyPath);
                await packagedDatabase.CopyToAsync(output, cancellationToken);
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
