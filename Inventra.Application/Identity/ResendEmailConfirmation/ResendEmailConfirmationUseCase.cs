namespace Inventra.Application.Identity.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationUseCase(
    IPasswordIdentityService passwordIdentityService)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        ResendEmailConfirmationRequest request,
        CancellationToken cancellationToken = default)
    {
        return await passwordIdentityService.ResendConfirmationAsync(
            request.Email,
            cancellationToken);
    }
}
