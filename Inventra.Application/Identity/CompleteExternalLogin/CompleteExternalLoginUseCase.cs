namespace Inventra.Application.Identity.CompleteExternalLogin;

public sealed class CompleteExternalLoginUseCase(
    IExternalIdentityService externalIdentityService,
    IAuthenticationSession authenticationSession,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result<UserProfileResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var externalUser = await externalIdentityService.CompleteSignInAsync(cancellationToken);

        if (externalUser is null)
            return IdentityErrors.ExternalLoginFailed();

        var user = await GetOrCreateUserAsync(externalUser, cancellationToken);

        if (user.IsBlocked)
            return await SignOutBlockedUserAsync();

        await unitOfWork.SaveChangesAsync(cancellationToken);
            
        return user.ToProfileResponse();
    }

    private async Task<UserAccount> GetOrCreateUserAsync(
        ExternalUserInfo externalUser,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(externalUser.UserId, cancellationToken);

        if (user is not null)
            return UpdateUser(user, externalUser);

        user = new UserAccount(externalUser.UserId, externalUser.UserName, externalUser.Email);
        await userRepository.AddAsync(user, cancellationToken);

        return user;
    }

    private static UserAccount UpdateUser(UserAccount user, ExternalUserInfo externalUser)
    {
        user.Rename(externalUser.UserName);
        user.ChangeEmail(externalUser.Email);

        return user;
    }

    private async Task<Result<UserProfileResponse>> SignOutBlockedUserAsync()
    {
        await authenticationSession.SignOutAsync();

        return IdentityErrors.UserBlocked();
    }
}
