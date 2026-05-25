namespace Inventra.Application.Identity.ConfirmEmail;

public sealed class ConfirmEmailUseCase(
    IPasswordIdentityService passwordIdentityService)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        return await passwordIdentityService.ConfirmEmailAsync(
            request.UserId,
            request.Token,
            cancellationToken);
    }
}
