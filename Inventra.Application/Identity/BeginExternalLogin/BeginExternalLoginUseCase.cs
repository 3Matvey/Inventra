namespace Inventra.Application.Identity.BeginExternalLogin;

public sealed class BeginExternalLoginUseCase(
    IExternalAuthenticationSchemeService schemeService)
    : IUseCase
{
    public async Task<Result<ExternalLoginChallengeResponse>> ExecuteAsync(
        BeginExternalLoginRequest request)
    {
        if (!await schemeService.IsConfiguredAsync(request.Provider))
            return IdentityErrors.ExternalProviderNotConfigured(request.Provider);

        return new ExternalLoginChallengeResponse(
            request.Provider,
            BuildCallbackUri(request.ReturnUrl));
    }

    private static string BuildCallbackUri(string returnUrl)
    {
        return "/auth/external-login/callback" +
               $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
    }
}
