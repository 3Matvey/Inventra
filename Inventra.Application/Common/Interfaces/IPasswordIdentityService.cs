using Inventra.Application.Identity;

namespace Inventra.Application.Common.Interfaces;

/// <summary>
/// Provides password-based identity operations.
/// </summary>
public interface IPasswordIdentityService
{
    Task<Result<PasswordIdentityUserInfo>> RegisterAsync(
        PasswordRegistrationRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> LoginAsync(
        PasswordLoginRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    Task<Result> ResendConfirmationAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result> SetPasswordAsync(
        SetPasswordRequest request,
        CancellationToken cancellationToken = default);
}
