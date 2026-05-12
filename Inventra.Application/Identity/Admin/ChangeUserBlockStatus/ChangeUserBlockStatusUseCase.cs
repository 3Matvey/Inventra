namespace Inventra.Application.Identity.Admin.ChangeUserBlockStatus;

public sealed class ChangeUserBlockStatusUseCase(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IIdentityAccountService identityAccountService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        ChangeUserBlockStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            return IdentityErrors.AdminRequired();

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return IdentityErrors.UserNotFound(request.UserId);

        ChangeBlockStatus(user, request.IsBlocked);
        await identityAccountService.SetBlockedAsync(user.Id, request.IsBlocked, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static void ChangeBlockStatus(Domain.Entities.UserAccount user, bool isBlocked)
    {
        if (isBlocked)
            user.Block();
        else
            user.Unblock();
    }
}
