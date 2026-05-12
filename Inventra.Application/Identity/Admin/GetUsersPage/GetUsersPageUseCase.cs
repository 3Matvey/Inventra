using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;

namespace Inventra.Application.Identity.Admin.GetUsersPage;

public sealed class GetUsersPageUseCase(
    ICurrentUser currentUser,
    IUserRepository userRepository)
    : IUseCase
{
    public async Task<Result<IReadOnlyCollection<UserProfileResponse>>> ExecuteAsync(
        GetUsersPageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            return IdentityErrors.AdminRequired();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var users = await userRepository.GetPageAsync(page, pageSize, cancellationToken);

        return users.Select(x => x.ToProfileResponse()).ToArray();
    }
}
