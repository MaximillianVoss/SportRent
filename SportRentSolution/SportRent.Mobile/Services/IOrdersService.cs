using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public interface IOrdersService
{
    Task<IReadOnlyList<UserOrder>> GetOrdersAsync(int userId, CancellationToken cancellationToken = default);

    Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task PayOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);

    Task CancelOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);
}
