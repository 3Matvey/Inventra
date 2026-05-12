using Inventra.Application.Common.Interfaces;
using Inventra.Application.Identity.CompleteExternalLogin;
using Inventra.Application.Identity.GetCurrentUserProfile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("auth")]
public class AuthController : ApiControllerBase
{
    [HttpGet("external/{provider}")]
    public IActionResult ExternalChallenge(string provider, string returnUrl = "/")
    {
        var redirectUrl = Url.Action(nameof(ExternalCallback), new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, provider);
    }

    [HttpGet("external/callback")]
    public async Task<IActionResult> ExternalCallback(
        [FromServices] CompleteExternalLoginUseCase useCase,
        string returnUrl = "/",
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);

        return result.IsSuccess ? LocalRedirect(returnUrl) : FromResult(result);
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
}
