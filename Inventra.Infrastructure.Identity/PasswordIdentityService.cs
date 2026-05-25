using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Inventra.Infrastructure.Identity;

internal sealed class PasswordIdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    SmtpEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration) : IPasswordIdentityService
{
    public async Task<Result<PasswordIdentityUserInfo>> RegisterAsync(
        PasswordRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
            return await RegisterExistingUserAsync(existingUser, cancellationToken);

        var user = CreateUser(request);
        var created = await userManager.CreateAsync(user, request.Password);

        if (!created.Succeeded)
            return IdentityErrors.RegistrationFailed(ToDescription(created));

        await SendConfirmationEmailAsync(user, cancellationToken);

        return ToUserInfo(user, shouldCreateUserAccount: true);
    }

    public async Task<Result> LoginAsync(
        PasswordLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return IdentityErrors.InvalidCredentials();

        return await SignInAsync(user, request);
    }

    public async Task<Result> ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return IdentityErrors.UserNotFound(userId);

        var result = await userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded ? Result.Success() : IdentityErrors.EmailConfirmationFailed();
    }

    public async Task<Result> ResendConfirmationAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || await userManager.IsEmailConfirmedAsync(user))
            return Result.Success();

        await SendConfirmationEmailAsync(user, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetPasswordAsync(
        SetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return IdentityErrors.UserNotFound(request.UserId);

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

        return result.Succeeded
            ? Result.Success()
            : IdentityErrors.SetPasswordFailed(ToDescription(result));
    }

    private async Task<Result<PasswordIdentityUserInfo>> RegisterExistingUserAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        if (await userManager.HasPasswordAsync(user))
            return IdentityErrors.RegistrationFailed("A user with this e-mail already exists.");

        await SendPasswordSetupEmailAsync(user, cancellationToken);

        return ToUserInfo(user, shouldCreateUserAccount: false);
    }

    private async Task<Result> SignInAsync(
        ApplicationUser user,
        PasswordLoginRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: true);

        return ToLoginResult(result);
    }

    private async Task SendConfirmationEmailAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = BuildConfirmationLink(user.Id, token);

        await emailSender.SendAsync(
            user.Email!,
            "Confirm your Inventra account",
            CreateConfirmationBody(link),
            cancellationToken);
    }

    private async Task SendPasswordSetupEmailAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var link = BuildPasswordSetupLink(user.Id, token);

        await emailSender.SendAsync(
            user.Email!,
            "Set password for your Inventra account",
            CreatePasswordSetupBody(link),
            cancellationToken);
    }

    private string BuildConfirmationLink(Guid userId, string token)
    {
        var request = CurrentRequest();

        return $"{request.Scheme}://{request.Host}/auth/confirm-email" +
               $"?userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    private string BuildPasswordSetupLink(Guid userId, string token)
    {
        var frontendBaseUrl = configuration["Frontend:BaseUrl"]!.TrimEnd('/');

        return $"{frontendBaseUrl}/login?setupPassword=true" +
               $"&userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    private HttpRequest CurrentRequest()
    {
        return httpContextAccessor.HttpContext?.Request
            ?? throw new InvalidOperationException("Current HTTP request is not available.");
    }

    private static ApplicationUser CreateUser(PasswordRegistrationRequest request)
    {
        return new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = false
        };
    }

    private static Result ToLoginResult(SignInResult result)
    {
        if (result.Succeeded)
            return Result.Success();

        if (result.IsLockedOut)
            return IdentityErrors.UserBlocked();

        return result.IsNotAllowed
            ? IdentityErrors.EmailNotConfirmed()
            : IdentityErrors.InvalidCredentials();
    }

    private static PasswordIdentityUserInfo ToUserInfo(
        ApplicationUser user,
        bool shouldCreateUserAccount)
    {
        return new PasswordIdentityUserInfo(
            user.Id,
            user.UserName!,
            user.Email!,
            shouldCreateUserAccount);
    }

    private static string CreateConfirmationBody(string link)
    {
        return $"""
                <p>Please confirm your Inventra account by clicking the link below.</p>
                <p><a href="{link}">Confirm e-mail</a></p>
                <p>{link}</p>
                """;
    }

    private static string CreatePasswordSetupBody(string link)
    {
        return $"""
                <p>Your Inventra account already exists through an external sign-in provider.</p>
                <p>Use the link below to add password sign-in to the same account.</p>
                <p><a href="{link}">Set password</a></p>
                <p>{link}</p>
                """;
    }

    private static string ToDescription(IdentityResult result)
    {
        return string.Join("; ", result.Errors.Select(x => x.Description));
    }
}
