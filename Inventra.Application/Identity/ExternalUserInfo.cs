namespace Inventra.Application.Identity;

public sealed record ExternalUserInfo(
    Guid UserId,
    string Provider,
    string ProviderUserId,
    string UserName,
    string Email);
