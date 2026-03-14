namespace SportRent.Mobile.Models;

public sealed class CatalogCategory
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }
}
