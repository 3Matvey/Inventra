namespace Inventra.Application.Identity;

/// <summary>
/// Contains credentials for password login.
/// </summary>
public sealed record PasswordLoginRequest(
    string Email,
    string Password,
    bool RememberMe);
