namespace Inventra.Application.Identity.SetPassword;

public sealed class SetPasswordUseCase(
    IPasswordIdentityService passwordIdentityService)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        SetPasswordUseCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return await passwordIdentityService.SetPasswordAsync(
            ToIdentityRequest(request),
            cancellationToken);
    }

    private static SetPasswordRequest ToIdentityRequest(
        SetPasswordUseCaseRequest request)
    {
        return new SetPasswordRequest(
            request.UserId,
            request.Token,
            request.Password);
    }
}
