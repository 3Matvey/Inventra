using Inventra.Application.Identity.BeginExternalLogin;
using Microsoft.AspNetCore.Authentication;

namespace Inventra.Api.Controllers.Responses;

internal static class ExternalLoginChallengeMapping
{
    public static AuthenticationProperties ToAuthenticationProperties(
        this ExternalLoginChallengeResponse challenge)
    {
        return new AuthenticationProperties
        {
            RedirectUri = challenge.RedirectUri,
            Items = { ["LoginProvider"] = challenge.Provider }
        };
    }
}
