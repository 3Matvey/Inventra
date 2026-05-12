using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;

namespace Inventra.Application.Identity.Admin.ChangeUserAdminRole;

public sealed class ChangeUserAdminRoleUseCase(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IIdentityAccountService identityAccountService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        ChangeUserAdminRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            return IdentityErrors.AdminRequired();

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return IdentityErrors.UserNotFound(request.UserId);

        ChangeRole(user, request.IsAdmin);
        await identityAccountService.SetAdminRoleAsync(user.Id, request.IsAdmin, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static void ChangeRole(Domain.Entities.UserAccount user, bool isAdmin)
    {
        if (isAdmin)
            user.AddAdminRole();
        else
            user.RemoveAdminRole();
    }
}
