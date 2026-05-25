namespace Inventra.Application.Identity;

/// <summary>
/// Describes an identity user created through password registration.
/// </summary>
public sealed record PasswordIdentityUserInfo(
    Guid UserId,
    string UserName,
    string Email,
    bool ShouldCreateUserAccount);
