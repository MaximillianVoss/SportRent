using SportRent.Mobile.Infrastructure;

namespace SportRent.Mobile.Models;

public sealed class UserOrder
{
    public required int Id { get; init; }

    public required string StatusTitle { get; init; }

    public required string IssuePointName { get; init; }

    public string? ReturnPointName { get; init; }

    public required string EquipmentSummary { get; init; }

    public string? Description { get; init; }

    public required DateTime DateCreated { get; init; }

    public required DateTime DateStart { get; init; }

    public required DateTime DateEnd { get; init; }

    public required int Amount { get; init; }

    public required int DepositAmount { get; init; }

    public required int TotalPaymentAmount { get; init; }

    public string? PaymentMethodTitle { get; init; }

    public string? PaymentStatusTitle { get; init; }

    public string ScheduleText => $"{DisplayFormatter.ToDateTime(DateStart)} - {DisplayFormatter.ToDateTime(DateEnd)}";

    public string CreatedText => $"Создан {DisplayFormatter.ToDateTime(DateCreated)}";

    public string AmountText => DisplayFormatter.ToCurrency(Amount);

    public string DepositText => DepositAmount > 0 ? DisplayFormatter.ToCurrency(DepositAmount) : "Без залога";

    public string TotalPaymentText => DisplayFormatter.ToCurrency(TotalPaymentAmount);

    public string PaymentText =>
        string.IsNullOrWhiteSpace(PaymentMethodTitle) || string.IsNullOrWhiteSpace(PaymentStatusTitle)
            ? "Платеж еще не зарегистрирован"
            : $"{PaymentMethodTitle} · {PaymentStatusTitle}";

    public bool IsPaymentPending =>
        string.Equals(PaymentStatusTitle, "Ожидает оплаты", StringComparison.OrdinalIgnoreCase);

    public string PointText =>
        string.IsNullOrWhiteSpace(ReturnPointName) || string.Equals(ReturnPointName, IssuePointName, StringComparison.Ordinal)
            ? IssuePointName
            : $"{IssuePointName} -> {ReturnPointName}";

    public string StatusBackgroundColor => StatusTitle switch
    {
        "Подтвержден" => "#DDEBFF",
        "В аренде" => "#E8F4D8",
        "Завершен" => "#DDEFE4",
        "Отменен" => "#F9DEDD",
        _ => "#F5E8D0"
    };

    public string StatusTextColor => StatusTitle switch
    {
        "Подтвержден" => "#255FAA",
        "В аренде" => "#5B7C2B",
        "Завершен" => "#1F6B53",
        "Отменен" => "#8C2F39",
        _ => "#8A5A12"
    };
}
