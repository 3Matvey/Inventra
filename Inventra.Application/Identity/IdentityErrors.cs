namespace Inventra.Application.Identity;

public static class IdentityErrors
{
    public static Error AuthenticationRequired() =>
        Error.AccessUnauthorized(
            "Identity.AuthenticationRequired",
            "Authentication is required.");

    public static Error ExternalLoginFailed() =>
        Error.AccessUnauthorized(
            "Identity.ExternalLoginFailed",
            "External login could not be completed.");

    public static Error ExternalProviderNotConfigured(string provider) =>
        Error.BadRequest(
            "Identity.ExternalProviderNotConfigured",
            $"External authentication provider '{provider}' is not configured.");

    public static Error InvalidCredentials() =>
        Error.AccessUnauthorized(
            "Identity.InvalidCredentials",
            "Invalid e-mail or password.");

    public static Error EmailNotConfirmed() =>
        Error.AccessForbidden(
            "Identity.EmailNotConfirmed",
            "E-mail address is not confirmed.");

    public static Error EmailConfirmationFailed() =>
        Error.BadRequest(
            "Identity.EmailConfirmationFailed",
            "E-mail confirmation could not be completed.");

    public static Error RegistrationFailed(string description) =>
        Error.BadRequest(
            "Identity.RegistrationFailed",
            description);

    public static Error UserBlocked() =>
        Error.AccessForbidden(
            "Identity.UserBlocked",
            "This user account is blocked.");

    public static Error AdminRequired() =>
        Error.AccessForbidden(
            "Identity.AdminRequired",
            "Admin access is required.");

    public static Error UserNotFound(Guid userId) =>
        Error.NotFound(
            "Identity.UserNotFound",
            $"User '{userId}' was not found.");
}
