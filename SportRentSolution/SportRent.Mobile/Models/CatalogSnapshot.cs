namespace SportRent.Mobile.Models;

public sealed class CatalogSnapshot
{
    public required IReadOnlyList<CatalogCategory> Categories { get; init; }

    public required IReadOnlyList<CatalogEquipmentItem> Equipment { get; init; }

    public required CatalogStats Stats { get; init; }
}
