using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

/// <summary>
/// Хранит текущую пользовательскую сессию в памяти приложения.
/// </summary>
public interface IUserSessionService
{
    /// <summary>
    /// Срабатывает при входе или выходе пользователя, чтобы приложение могло сменить корневой экран.
    /// </summary>
    event EventHandler? SessionChanged;

    /// <summary>
    /// Текущий авторизованный пользователь или null, если вход не выполнен.
    /// </summary>
    UserSession? CurrentUser { get; }

    /// <summary>
    /// Показывает, есть ли активная пользовательская сессия.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Устанавливает текущего пользователя после успешной авторизации.
    /// </summary>
    void SetCurrentUser(UserSession user);

    /// <summary>
    /// Завершает текущую пользовательскую сессию.
    /// </summary>
    void SignOut();
}
