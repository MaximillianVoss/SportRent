namespace SportRent.Mobile.Models;

public sealed class CreateOrderRequest
{
    public required int UserId { get; init; }

    public required int EquipmentId { get; init; }

    public required string EquipmentTitle { get; init; }

    public required int RentalPointEquipmentId { get; init; }

    public required int Quantity { get; init; }

    public required int UnitHours { get; init; }

    public required int PeriodCount { get; init; }

    public required int PricePerUnit { get; init; }

    public required int DepositPerUnit { get; init; }

    public required DateTime StartAt { get; init; }

    public string? Description { get; init; }
}
