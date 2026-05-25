namespace Inventra.Application.Identity.LoginWithPassword;

public sealed class LoginWithPasswordUseCase(
    IPasswordIdentityService passwordIdentityService)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        LoginWithPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        return await passwordIdentityService.LoginAsync(
            ToIdentityRequest(request),
            cancellationToken);
    }

    private static PasswordLoginRequest ToIdentityRequest(
        LoginWithPasswordRequest request)
    {
        return new PasswordLoginRequest(
            request.Email,
            request.Password,
            request.RememberMe);
    }
}
