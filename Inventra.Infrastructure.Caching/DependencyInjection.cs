using Inventra.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Infrastructure.Caching;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCachingServices(IConfiguration configuration)
        {
            var options = GetOptions(configuration);

            services.AddStackExchangeRedisCache(redis =>
            {
                redis.Configuration = options.ConnectionString;
            });

            services.AddScoped<IBlockedUserSessionCache, RedisBlockedUserSessionCache>();

            return services;
        }
    }

    private static CachingOptions GetOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection(CachingOptions.SectionName);
        var options = new CachingOptions
        {
            ConnectionString = section[nameof(CachingOptions.ConnectionString)] ?? string.Empty
        };

        return !string.IsNullOrWhiteSpace(options.ConnectionString)
            ? options
            : throw new InvalidOperationException("Redis:ConnectionString is not configured.");
    }
}
