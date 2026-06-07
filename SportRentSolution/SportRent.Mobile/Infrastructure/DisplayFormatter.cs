using System.Globalization;

namespace SportRent.Mobile.Infrastructure;

/// <summary>
/// Форматирует даты, суммы и длительности для отображения в интерфейсе.
/// </summary>
internal static class DisplayFormatter
{
    private static readonly CultureInfo RussianCulture = CultureInfo.GetCultureInfo("ru-RU");

    /// <summary>
    /// Преобразует сумму из копеек в строку с рублями.
    /// </summary>
    public static string ToCurrency(int amountInKopecks)
    {
        decimal rubles = amountInKopecks / 100m;
        return string.Format(RussianCulture, "{0:N0} ₽", rubles);
    }

    /// <summary>
    /// Возвращает человекочитаемое название периода аренды.
    /// </summary>
    public static string ToDuration(int unitHours)
    {
        return unitHours switch
        {
            1 => "1 час",
            24 => "1 день",
            _ => $"{unitHours} ч."
        };
    }

    /// <summary>
    /// Форматирует дату и время для карточек заказов.
    /// </summary>
    public static string ToDateTime(DateTime value)
    {
        return value.ToString("dd MMM yyyy, HH:mm", RussianCulture);
    }

    /// <summary>
    /// Форматирует дату без времени для профиля пользователя.
    /// </summary>
    public static string ToDate(DateTime value)
    {
        return value.ToString("dd MMMM yyyy", RussianCulture);
    }
}
