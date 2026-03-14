using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public sealed class UserSessionService : IUserSessionService
{
    public event EventHandler? SessionChanged;

    public UserSession? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public void SetCurrentUser(UserSession user)
    {
        CurrentUser = user;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SignOut()
    {
        if (CurrentUser is null)
        {
            return;
        }

        CurrentUser = null;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }
}
