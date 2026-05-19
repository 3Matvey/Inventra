namespace Inventra.Infrastructure.Identity;

public interface IBlockedUserSessionCache
{
    Task MarkBlockedAsync(Guid userId, CancellationToken cancellationToken = default);

    Task MarkUnblockedAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> IsBlockedAsync(Guid userId, CancellationToken cancellationToken = default);
}
