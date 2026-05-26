using SportRent.Mobile.Infrastructure;

namespace SportRent.Mobile.Models;

public sealed class UserProfile
{
    public required int UserId { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }

    public required string RoleTitle { get; init; }

    public required DateTime DateCreated { get; init; }

    public required int TotalOrders { get; init; }

    public required int ActiveOrders { get; init; }

    public required int CompletedOrders { get; init; }

    public required int TotalPaidAmount { get; init; }

    public required int OutstandingAmount { get; init; }

    public string DisplayName => $"{FirstName} {LastName}";

    public string MemberSinceText => $"В системе с {DisplayFormatter.ToDate(DateCreated)}";

    public string TotalPaidText => DisplayFormatter.ToCurrency(TotalPaidAmount);

    public string OutstandingText => OutstandingAmount > 0 ? DisplayFormatter.ToCurrency(OutstandingAmount) : "Нет задолженности";
}
