using SportRent.Mobile.Infrastructure;

namespace SportRent.Mobile.Models;

public sealed class CatalogEquipmentItem
{
    public required int Id { get; init; }

    public required int CategoryId { get; init; }

    public required string Title { get; init; }

    public required string CategoryTitle { get; init; }

    public required string BrandTitle { get; init; }

    public required string TypeTitle { get; init; }

    public string? Model { get; init; }

    public string? Description { get; init; }

    public string? ImageUrl { get; init; }

    public required int StartingPrice { get; init; }

    public required string StartingRentalTypeTitle { get; init; }

    public required int Deposit { get; init; }

    public required int AvailableUnits { get; init; }

    public required int RentalPointCount { get; init; }

    public required string AccentColor { get; init; }

    public required string AccentSurfaceColor { get; init; }

    public required string SymbolText { get; init; }

    public string BrandModelText => string.IsNullOrWhiteSpace(Model) ? BrandTitle : $"{BrandTitle} {Model}";

    public string DescriptionText => string.IsNullOrWhiteSpace(Description) ? $"{TypeTitle} доступен для аренды." : Description!;

    public string PriceText =>
        StartingPrice > 0
            ? $"от {DisplayFormatter.ToCurrency(StartingPrice)} / {StartingRentalTypeTitle.ToLowerInvariant()}"
            : "Цена по запросу";

    public string DepositText => Deposit > 0 ? $"Залог {DisplayFormatter.ToCurrency(Deposit)}" : "Без залога";

    public string AvailabilityText =>
        AvailableUnits > 0
            ? $"{AvailableUnits} ед. в {RentalPointCount} пунктах"
            : "Свободных единиц сейчас нет";
}
