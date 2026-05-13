using Inventra.Application.Items.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryItemQueries
{
    private sealed class ItemRowBase
    {
        public Guid Id { get; init; }
        public Guid InventoryId { get; init; }
        public string CustomId { get; init; } = string.Empty;
        public long? SequenceNumber { get; init; }
        public Guid CreatedById { get; init; }
        public string CreatedByUserName { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public long Version { get; init; }
        public int LikesCount { get; init; }
        public bool IsLikedByCurrentUser { get; init; }
    }

    private sealed class ItemFieldValueRow
    {
        public Guid ItemId { get; init; }
        public ItemFieldValueDto Value { get; init; } = null!;
    }

    private sealed class StringFrequencyRow
    {
        public Guid FieldId { get; init; }
        public string FieldTitle { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
        public int Count { get; init; }
    }
}
