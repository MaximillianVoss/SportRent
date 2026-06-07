using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

/// <summary>
/// Описывает операции авторизации и получения данных профиля пользователя.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Возвращает демонстрационные учетные записи для быстрого входа при проверке приложения.
    /// </summary>
    Task<IReadOnlyList<DemoAccount>> GetDemoAccountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет почту и пароль пользователя по локальной базе данных.
    /// </summary>
    Task<UserSession?> SignInAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает профиль пользователя вместе с агрегированной статистикой заказов и платежей.
    /// </summary>
    Task<UserProfile?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
}
