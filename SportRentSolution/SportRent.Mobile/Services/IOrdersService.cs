using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public interface IOrdersService
{
    /// <summary>
    /// Возвращает историю заказов аренды для конкретного пользователя.
    /// </summary>
    Task<IReadOnlyList<UserOrder>> GetOrdersAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает заказ со статусом ожидания оплаты и резервирует выбранный остаток инвентаря.
    /// </summary>
    Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отмечает тестовый платеж как оплаченный и подтверждает созданный заказ.
    /// </summary>
    Task PayOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отменяет созданный неоплаченный заказ и возвращает зарезервированный остаток в каталог.
    /// </summary>
    Task CancelOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);
}
