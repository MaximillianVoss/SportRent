using SportRent.Mobile.Models;

namespace SportRent.Mobile.Services;

public interface IOrdersService
{
    /// <summary>
    /// Gets the rental order history for a concrete user.
    /// </summary>
    Task<IReadOnlyList<UserOrder>> GetOrdersAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a pending rental order and reserves the selected stock quantity.
    /// </summary>
    Task<int> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a pending order payment as paid and confirms the created order.
    /// </summary>
    Task PayOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a created unpaid order and restores the reserved catalog stock.
    /// </summary>
    Task CancelOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);
}
