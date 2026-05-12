namespace Inventra.Application.Identity;

internal static class UserProfileMapping
{
    public static UserProfileResponse ToProfileResponse(this UserAccount user)
    {
        return new UserProfileResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.IsBlocked,
            user.IsAdmin);
    }
}
