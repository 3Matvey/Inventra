namespace Inventra.Application.Identity;

public sealed record UserProfileResponse(
    Guid Id,
    string UserName,
    string Email,
    bool IsBlocked,
    bool IsAdmin);
