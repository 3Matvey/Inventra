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
        var linkedUser = await FindLinkedUserAsync(loginInfo);

        if (linkedUser is not null)
            return await UpdateFallbackEmailAsync(linkedUser, loginInfo);

        var emailUser = await FindUserByEmailAsync(loginInfo);

        return emailUser is not null
            ? await LinkLoginAsync(emailUser, loginInfo)
            : await CreateAndLinkUserAsync(loginInfo);
    }

    private Task<ApplicationUser?> FindLinkedUserAsync(ExternalLoginInfo loginInfo)
    {
        return userManager.FindByLoginAsync(
            loginInfo.LoginProvider,
            loginInfo.ProviderKey);
    }

    private async Task<ApplicationUser?> FindUserByEmailAsync(ExternalLoginInfo loginInfo)
    {
        return await userManager.FindByEmailAsync(GetEmail(loginInfo));
    }

    private async Task<ApplicationUser?> LinkLoginAsync(
        ApplicationUser user,
        ExternalLoginInfo loginInfo)
    {
        var linked = await userManager.AddLoginAsync(user, loginInfo);

        return linked.Succeeded ? user : LogAndReturnNull("External login link", loginInfo, linked);
    }

    private async Task<ApplicationUser?> CreateAndLinkUserAsync(ExternalLoginInfo loginInfo)
    {
        var user = await CreateUser(loginInfo);
        var created = await userManager.CreateAsync(user);

        if (!created.Succeeded)
            return LogAndReturnNull("External user creation", loginInfo, created);

        return await LinkLoginAsync(user, loginInfo);
    }

    private async Task<ApplicationUser> CreateUser(ExternalLoginInfo loginInfo)
    {
        var email = GetEmail(loginInfo);

        return new ApplicationUser
        {
            UserName = GetUserName(loginInfo, email),
            Email = email,
            EmailConfirmed = true
        };
    }

    private async Task<ApplicationUser?> UpdateFallbackEmailAsync(
        ApplicationUser user,
        ExternalLoginInfo loginInfo)
    {
        if (!ShouldUpdateEmail(user.Email, loginInfo))
            return user;

        user.Email = GetEmail(loginInfo);
        user.EmailConfirmed = true;
        var updated = await userManager.UpdateAsync(user);

        return updated.Succeeded ? user : LogAndReturnNull("External user update", loginInfo, updated);
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

    private ApplicationUser? LogAndReturnNull(
        string operation,
        ExternalLoginInfo loginInfo,
        IdentityResult result)
    {
        logger.LogWarning(
            "{Operation} failed for provider {Provider}. Errors: {Errors}.",
            operation,
            loginInfo.LoginProvider,
            string.Join("; ", result.Errors.Select(error => error.Description)));

        return null;
    }

    private static string GetEmail(ExternalLoginInfo loginInfo)
    {
        return loginInfo.Principal.FindFirstValue(ClaimTypes.Email)
            ?? BuildFallbackEmail(loginInfo);
    }

    private static bool ShouldUpdateEmail(string? currentEmail, ExternalLoginInfo loginInfo)
    {
        return IsFallbackEmail(currentEmail)
            && !IsFallbackEmail(GetEmail(loginInfo));
    }

    private static bool IsFallbackEmail(string? email)
    {
        return email?.EndsWith(".external", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string GetUserName(ExternalLoginInfo loginInfo, string email)
    {
        return IsFallbackEmail(email)
            ? $"{loginInfo.LoginProvider}{loginInfo.ProviderKey}"
            : email;
    }

    private static string BuildFallbackEmail(ExternalLoginInfo loginInfo)
    {
        return $"{loginInfo.ProviderKey}@{loginInfo.LoginProvider}.external";
    }
}
