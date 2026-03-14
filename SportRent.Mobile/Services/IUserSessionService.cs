using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public interface IUserSessionService
{
    event EventHandler? SessionChanged;

    UserSession? CurrentUser { get; }

    bool IsAuthenticated { get; }

    void SetCurrentUser(UserSession user);

    void SignOut();
}
