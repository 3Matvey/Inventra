namespace Inventra.Application.Inventories.CustomIds;

internal sealed record CustomIdComposeContext(
    DateTimeOffset CreatedAt,
    long? Sequence,
    bool UseSampleValues);
