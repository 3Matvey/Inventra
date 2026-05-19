namespace Inventra.Infrastructure.Caching;

internal sealed class CachingOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; init; } = string.Empty;
}
