namespace SportRent.Mobile.Infrastructure;

/// <summary>
/// Централизованные маршруты Shell, используемые обработчиками навигации.
/// </summary>
public static class AppRoutes
{
    /// <summary>
    /// Абсолютный маршрут вкладки каталога для явных кнопок возврата.
    /// </summary>
    public const string Catalog = "//catalog";

    /// <summary>
    /// Абсолютный маршрут вкладки заказов после успешного оформления аренды.
    /// </summary>
    public const string Orders = "//orders";
}
