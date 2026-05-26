namespace SportRent.Mobile.Models;

public sealed class UserSession
{
    public required int UserId { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required string Phone { get; init; }

    public required string RoleTitle { get; init; }

    public string DisplayName => $"{FirstName} {LastName}";

    public string ShortName => $"{FirstName} {LastName[..1]}.";
}
