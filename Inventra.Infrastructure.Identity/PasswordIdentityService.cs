using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Inventra.Infrastructure.Identity;

internal sealed class PasswordIdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    SmtpEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : IPasswordIdentityService
{
    public async Task<Result<PasswordIdentityUserInfo>> RegisterAsync(
        PasswordRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = CreateUser(request);
        var created = await userManager.CreateAsync(user, request.Password);

        if (!created.Succeeded)
            return IdentityErrors.RegistrationFailed(ToDescription(created));

        await SendConfirmationEmailAsync(user, cancellationToken);

        return new PasswordIdentityUserInfo(user.Id, user.UserName!, user.Email!);
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

    private string BuildConfirmationLink(Guid userId, string token)
    {
        var request = CurrentRequest();

        return $"{request.Scheme}://{request.Host}/auth/confirm-email" +
               $"?userId={userId}&token={Uri.EscapeDataString(token)}";
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

    private static string CreateConfirmationBody(string link)
    {
        return $"""
                <p>Please confirm your Inventra account by clicking the link below.</p>
                <p><a href="{link}">Confirm e-mail</a></p>
                <p>{link}</p>
                """;
    }

    private static string ToDescription(IdentityResult result)
    {
        return string.Join("; ", result.Errors.Select(x => x.Description));
    }
}
