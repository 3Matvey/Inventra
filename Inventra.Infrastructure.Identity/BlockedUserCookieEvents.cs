using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Inventra.Infrastructure.Identity;

internal sealed class BlockedUserCookieEvents(
    IBlockedUserSessionCache blockedUsers) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userId = GetUserId(context.Principal);

        if (userId is null)
            return; //TODO подумать кто будет решать логаут

        if (!await blockedUsers.IsBlockedAsync(userId.Value, context.HttpContext.RequestAborted))
            return;

        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }

    private static Guid? GetUserId(ClaimsPrincipal? principal)
    {
        var value = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
