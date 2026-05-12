namespace Inventra.Application.Identity.Admin.DeleteUser;

public sealed class DeleteUserUseCase(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IIdentityAccountService identityAccountService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        DeleteUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            return IdentityErrors.AdminRequired();

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return IdentityErrors.UserNotFound(request.UserId);

        userRepository.Remove(user);
        await identityAccountService.DeleteAsync(user.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
