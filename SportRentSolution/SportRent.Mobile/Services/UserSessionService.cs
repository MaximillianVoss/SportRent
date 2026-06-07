using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

/// <summary>
/// Сервис текущей сессии пользователя, хранящий состояние в памяти приложения.
/// </summary>
public sealed class UserSessionService : IUserSessionService
{
    /// <inheritdoc />
    public event EventHandler? SessionChanged;

    /// <inheritdoc />
    public UserSession? CurrentUser { get; private set; }

    /// <inheritdoc />
    public bool IsAuthenticated => CurrentUser is not null;

    /// <inheritdoc />
    public void SetCurrentUser(UserSession user)
    {
        CurrentUser = user;
        // Уведомляем App, чтобы он переключил корневую страницу с LoginPage на AppShell.
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void SignOut()
    {
        if (CurrentUser is null)
        {
            return;
        }

        CurrentUser = null;
        // После выхода App снова покажет экран авторизации.
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }
}
