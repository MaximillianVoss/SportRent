namespace SportRent.Mobile.Infrastructure;

/// <summary>
/// Central Shell route names used by screen navigation handlers.
/// </summary>
public static class AppRoutes
{
    /// <summary>
    /// Absolute route to the catalog tab, used by explicit return-to-catalog buttons.
    /// </summary>
    public const string Catalog = "//catalog";

    /// <summary>
    /// Absolute route to the order history tab after successful order creation.
    /// </summary>
    public const string Orders = "//orders";
}
