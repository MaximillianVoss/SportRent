namespace SportRent.Mobile.Infrastructure;

/// <summary>
/// Подбирает цветовую идентичность карточки инвентаря по категории и типу.
/// </summary>
internal static class EquipmentVisualIdentity
{
    /// <summary>
    /// Возвращает акцентный цвет, цвет поверхности и короткий символ для карточки.
    /// </summary>
    public static (string AccentColor, string AccentSurfaceColor, string SymbolText) Resolve(string categoryTitle, string typeTitle)
    {
        string normalized = $"{categoryTitle} {typeTitle}".ToLowerInvariant();

        // Простые правила позволяют визуально различать типы инвентаря без дополнительных полей в базе.
        if (normalized.Contains("вело"))
        {
            return ("#1F6B53", "#D9F0E6", "VE");
        }

        if (normalized.Contains("лыж"))
        {
            return ("#255FAA", "#DDEBFF", "LY");
        }

        if (normalized.Contains("сноуб"))
        {
            return ("#A5531E", "#FCE5D6", "SN");
        }

        if (normalized.Contains("самокат"))
        {
            return ("#5B7C2B", "#E6F0D7", "SK");
        }

        if (normalized.Contains("шлем") || normalized.Contains("защит"))
        {
            return ("#A2354C", "#F7DDE4", "ZT");
        }

        return ("#3D5A80", "#E1EAF4", "SR");
    }
}
