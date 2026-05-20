using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Identity;
using Inventra.Application.Identity.CompleteExternalLogin;
using Inventra.Application.Identity.GetCurrentUserProfile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("auth")]
public class AuthController(IConfiguration configuration) : ApiControllerBase
{
    [HttpGet("external/{provider}")]
    public async Task<IActionResult> ExternalChallenge(
        string provider,
        [FromServices] IAuthenticationSchemeProvider schemes,
        string returnUrl = "/")
    {
        if (await schemes.GetSchemeAsync(provider) is null)
            return BadRequest($"External authentication provider '{provider}' is not configured.");

        var redirectUrl = Url.Action(nameof(ExternalCallback), new { returnUrl });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            Items = { ["LoginProvider"] = provider }
        };

        return Challenge(properties, provider);
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
