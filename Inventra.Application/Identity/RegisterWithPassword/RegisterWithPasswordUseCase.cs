namespace Inventra.Application.Identity.RegisterWithPassword;

public sealed class RegisterWithPasswordUseCase(
    IPasswordIdentityService passwordIdentityService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result<Guid>> ExecuteAsync(
        RegisterWithPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await passwordIdentityService.RegisterAsync(
            ToIdentityRequest(request),
            cancellationToken);

        return result.IsSuccess
            ? await CompleteRegistrationAsync(result.Value, cancellationToken)
            : result.Error;
    }

    private async Task<Result<Guid>> CompleteRegistrationAsync(
        PasswordIdentityUserInfo identityUser,
        CancellationToken cancellationToken)
    {
        return identityUser.ShouldCreateUserAccount
            ? await CreateUserAccountAsync(identityUser, cancellationToken)
            : identityUser.UserId;
    }

    private async Task<Result<Guid>> CreateUserAccountAsync(
        PasswordIdentityUserInfo identityUser,
        CancellationToken cancellationToken)
    {
        var user = new UserAccount(
            identityUser.UserId,
            identityUser.UserName,
            identityUser.Email);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    private static PasswordRegistrationRequest ToIdentityRequest(
        RegisterWithPasswordRequest request)
    {
        return new PasswordRegistrationRequest(
            request.UserName,
            request.Email,
            request.Password);
    }
}
