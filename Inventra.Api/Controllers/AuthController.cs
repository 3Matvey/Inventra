using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Identity.BeginExternalLogin;
using Inventra.Application.Identity.ConfirmEmail;
using Inventra.Application.Identity;
using Inventra.Application.Identity.CompleteExternalLogin;
using Inventra.Application.Identity.GetCurrentUserProfile;
using Inventra.Application.Identity.LoginWithPassword;
using Inventra.Application.Identity.RegisterWithPassword;
using Inventra.Application.Identity.ResendEmailConfirmation;
using Inventra.Application.Identity.SetPassword;
using Inventra.Api.Controllers.Requests;
using Inventra.Api.Controllers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("auth")]
public class AuthController(IConfiguration configuration) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterWithPasswordBody body,
        [FromServices] RegisterWithPasswordUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(body.ToRequest(), cancellationToken);

        return FromResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginWithPasswordBody body,
        [FromServices] LoginWithPasswordUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(body.ToRequest(), cancellationToken);

        return FromResult(result);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromServices] ConfirmEmailUseCase useCase,
        Guid userId,
        string token,
        string returnUrl = "/login?emailConfirmed=true",
        CancellationToken cancellationToken = default)
    {
        var request = new ConfirmEmailRequest(userId, token);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return result.Match(
            () => Redirect(BuildFrontendRedirectUrl(returnUrl)),
            FromError);
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(
        [FromBody] ResendEmailConfirmationBody body,
        [FromServices] ResendEmailConfirmationUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(body.ToRequest(), cancellationToken);

        return FromResult(result);
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword(
        [FromBody] SetPasswordBody body,
        [FromServices] SetPasswordUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(body.ToRequest(), cancellationToken);

        return FromResult(result);
    }

    [HttpGet("external/{provider}")]
    public async Task<IActionResult> ExternalChallenge(
        string provider,
        [FromServices] BeginExternalLoginUseCase useCase,
        string returnUrl = "/")
    {
        var request = new BeginExternalLoginRequest(provider, returnUrl);
        var result = await useCase.ExecuteAsync(request);

        return result.Match(
            challenge => Challenge(challenge.ToAuthenticationProperties(), challenge.Provider),
            FromError);
    }

    [HttpGet("external-login/callback")]
    public async Task<IActionResult> ExternalCallback(
        [FromServices] CompleteExternalLoginUseCase useCase,
        string returnUrl = "/",
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);

        return result.Match(
            _ => Redirect(BuildFrontendRedirectUrl(returnUrl)),
            FromError);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromServices] IAuthenticationSession session)
    {
        await session.SignOutAsync();

        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(
        [FromServices] GetCurrentUserProfileUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);

        return FromResult(result);
    }

    private string BuildFrontendRedirectUrl(string returnUrl)
    {
        var safeReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        var frontendBaseUrl = configuration["Frontend:BaseUrl"]!.TrimEnd('/');

        return $"{frontendBaseUrl}/{safeReturnUrl.TrimStart('/')}";
    }
}
