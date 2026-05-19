using Inventra.Infrastructure.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Inventra.Infrastructure.Caching;

internal sealed class RedisBlockedUserSessionCache(
    IDistributedCache cache) : IBlockedUserSessionCache
{
    private const string KeyPrefix = "identity:blocked-users:";

    public Task MarkBlockedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return cache.SetStringAsync(Key(userId), "1", cancellationToken);
    }

    public Task MarkUnblockedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(Key(userId), cancellationToken);
    }

    public async Task<bool> IsBlockedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await cache.GetStringAsync(Key(userId), cancellationToken) is not null;
    }

    private static string Key(Guid userId) => $"{KeyPrefix}{userId:N}";
}
