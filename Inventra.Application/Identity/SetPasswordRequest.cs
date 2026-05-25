namespace Inventra.Application.Identity;

/// <summary>
/// Contains data required to add password credentials by token.
/// </summary>
public sealed record SetPasswordRequest(
    Guid UserId,
    string Token,
    string Password);
