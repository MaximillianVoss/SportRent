using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public interface IAuthenticationService
{
    Task<IReadOnlyList<DemoAccount>> GetDemoAccountsAsync(CancellationToken cancellationToken = default);

    Task<UserSession?> SignInAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<UserProfile?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
}
