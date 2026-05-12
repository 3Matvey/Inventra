namespace Inventra.Application.Common.Interfaces;

public interface IIdentityAccountService
{
    Task SetAdminRoleAsync(Guid userId, bool isAdmin, CancellationToken cancellationToken = default);

    Task SetBlockedAsync(Guid userId, bool isBlocked, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
}
