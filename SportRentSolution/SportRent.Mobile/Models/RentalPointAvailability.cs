namespace SportRent.Mobile.Models;

public sealed class RentalPointAvailability
{
    public required int Id { get; init; }

    public required int RentalPointId { get; init; }

    public required string RentalPointName { get; init; }

    public required string Address { get; init; }

    public string? Phone { get; init; }

    public required string ConditionTitle { get; init; }

    public required string SizeTitle { get; init; }

    public required int AvailableQuantity { get; init; }

    public required int TotalQuantity { get; init; }

    public bool IsAvailable => AvailableQuantity > 0;

    public string AvailabilityText => $"{AvailableQuantity} из {TotalQuantity} свободно";

    public string MetaText => $"Размер {SizeTitle} · {ConditionTitle}";

    public string PhoneText => string.IsNullOrWhiteSpace(Phone) ? "Телефон уточняется" : Phone!;
}
