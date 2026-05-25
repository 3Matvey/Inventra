namespace Inventra.Application.Identity;

/// <summary>
/// Contains data required to register a password-based account.
/// </summary>
public sealed record PasswordRegistrationRequest(
    string UserName,
    string Email,
    string Password);
