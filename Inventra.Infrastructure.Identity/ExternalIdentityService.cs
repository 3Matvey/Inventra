using System.Security.Claims;
using Inventra.Application.Common.Interfaces;
using Inventra.Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Inventra.Infrastructure.Identity;

internal class ExternalIdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<ExternalIdentityService> logger) : IExternalIdentityService
{
    public async Task<ExternalUserInfo?> CompleteSignInAsync(
        CancellationToken cancellationToken = default)
    {
        var loginInfo = await signInManager.GetExternalLoginInfoAsync();

        if (loginInfo is null)
            return null;

        var user = await GetOrCreateUserAsync(loginInfo);

        if (user is null)
            return null;

        await signInManager.SignInAsync(user, isPersistent: false, loginInfo.LoginProvider);

        return ToExternalUserInfo(user, loginInfo);
    }

    private async Task<ApplicationUser?> GetOrCreateUserAsync(ExternalLoginInfo loginInfo)
    {
        var user = await userManager.FindByLoginAsync(
            loginInfo.LoginProvider,
            loginInfo.ProviderKey);

        return user ?? await CreateUserAsync(loginInfo);
    }

    private async Task<ApplicationUser?> CreateUserAsync(ExternalLoginInfo loginInfo)
    {
        var email = GetEmail(loginInfo);
        var userName = GetUserName(loginInfo, email);
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = userName,
            Email = email,
            EmailConfirmed = true
        };

        var created = await userManager.CreateAsync(user);

        if (!created.Succeeded)
        {
            logger.LogWarning(
                "External user creation failed for provider {Provider}. Errors: {Errors}.",
                loginInfo.LoginProvider,
                string.Join("; ", created.Errors.Select(error => error.Description)));

            return null;
        }

        var linked = await userManager.AddLoginAsync(user, loginInfo);

        if (!linked.Succeeded)
        {
            logger.LogWarning(
                "External login link failed for provider {Provider}. Errors: {Errors}.",
                loginInfo.LoginProvider,
                string.Join("; ", linked.Errors.Select(error => error.Description)));
        }

        return linked.Succeeded ? user : null;
    }

    private static ExternalUserInfo ToExternalUserInfo(
        ApplicationUser user,
        ExternalLoginInfo loginInfo)
    {
        return new ExternalUserInfo(
            user.Id,
            loginInfo.LoginProvider,
            loginInfo.ProviderKey,
            user.UserName!,
            user.Email!);
    }

    private static string GetEmail(ExternalLoginInfo loginInfo)
    {
        return loginInfo.Principal.FindFirstValue(ClaimTypes.Email)
            ?? $"{loginInfo.ProviderKey}@{loginInfo.LoginProvider}.external";   //TODO 
    }

    private static string GetUserName(ExternalLoginInfo loginInfo, string email)
    {
        return loginInfo.Principal.FindFirstValue(ClaimTypes.Name)
            ?? email;
    }
}
