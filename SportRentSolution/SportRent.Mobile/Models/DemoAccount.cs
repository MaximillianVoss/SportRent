namespace SportRent.Mobile.Models;

public sealed class DemoAccount
{
    public required int UserId { get; init; }

    public required string DisplayName { get; init; }

    public required string RoleTitle { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public string Subtitle => $"{RoleTitle} · {Email}";
}
