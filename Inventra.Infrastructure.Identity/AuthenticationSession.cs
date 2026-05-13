using Inventra.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Inventra.Infrastructure.Identity;

internal class AuthenticationSession(
    IHttpContextAccessor httpContextAccessor,
    SignInManager<ApplicationUser> signInManager) : IAuthenticationSession
{
    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();

        if (httpContextAccessor.HttpContext is not null)
            await httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }
}
