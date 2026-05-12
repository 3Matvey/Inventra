using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;

namespace Inventra.Application.Identity.GetCurrentUserProfile;

public sealed class GetCurrentUserProfileUseCase(
    ICurrentUser currentUser,
    IUserRepository userRepository)
    : IUseCase
{
    public async Task<Result<UserProfileResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return IdentityErrors.AuthenticationRequired();

        var user = await userRepository.GetByIdAsync(currentUser.UserId.Value, cancellationToken);

        return user is null
            ? IdentityErrors.UserNotFound(currentUser.UserId.Value)
            : user.ToProfileResponse();
    }
}
