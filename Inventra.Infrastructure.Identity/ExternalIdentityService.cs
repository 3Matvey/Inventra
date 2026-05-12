using System.Security.Claims;
using Inventra.Application.Common.Interfaces;
using Inventra.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace Inventra.Infrastructure.Identity;

public class ExternalIdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IExternalIdentityService
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
            return null;

        var linked = await userManager.AddLoginAsync(user, loginInfo);

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
            ?? $"{loginInfo.ProviderKey}@{loginInfo.LoginProvider}.external";
    }

    private static string GetUserName(ExternalLoginInfo loginInfo, string email)
    {
        return loginInfo.Principal.FindFirstValue(ClaimTypes.Name)
            ?? email;
    }
}
