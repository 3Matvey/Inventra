using Inventra.Application.Common.Results;

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
