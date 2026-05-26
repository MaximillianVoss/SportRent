using System.Globalization;

namespace SportRent.Mobile.Infrastructure;

internal static class DisplayFormatter
{
    private static readonly CultureInfo RussianCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static string ToCurrency(int amountInKopecks)
    {
        decimal rubles = amountInKopecks / 100m;
        return string.Format(RussianCulture, "{0:N0} ₽", rubles);
    }

    public static string ToDuration(int unitHours)
    {
        return unitHours switch
        {
            1 => "1 час",
            24 => "1 день",
            _ => $"{unitHours} ч."
        };
    }

    public static string ToDateTime(DateTime value)
    {
        return value.ToString("dd MMM yyyy, HH:mm", RussianCulture);
    }

    public static string ToDate(DateTime value)
    {
        return value.ToString("dd MMMM yyyy", RussianCulture);
    }
}
