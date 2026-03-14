namespace SportRent.Mobile.Services;

public interface ILocalDatabaseService
{
    Task<string> GetWorkingCopyPathAsync(CancellationToken cancellationToken = default);
}
