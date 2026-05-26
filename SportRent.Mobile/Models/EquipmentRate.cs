using SportRent.Mobile.Infrastructure;

namespace SportRent.Mobile.Models;

public sealed class EquipmentRate
{
    public required string Title { get; init; }

    public required string Code { get; init; }

    public required int UnitHours { get; init; }

    public required int Price { get; init; }

    public required int Deposit { get; init; }

    public string RentalTypeTitle => Title;

    public string DurationText => DisplayFormatter.ToDuration(UnitHours);

    public string PriceText => DisplayFormatter.ToCurrency(Price);

    public string DepositText => Deposit > 0 ? $"Залог {DisplayFormatter.ToCurrency(Deposit)}" : "Без залога";
}
