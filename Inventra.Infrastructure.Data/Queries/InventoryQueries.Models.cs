using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryQueries
{
    private sealed class InventoryRowBase
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? DescriptionMarkdown { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImagePublicId { get; init; }
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public string OwnerName { get; init; } = string.Empty;
        public int ItemsCount { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
    }

    private sealed class InventoryTagRow
    {
        public Guid InventoryId { get; init; }
        public TagDto Tag { get; init; } = null!;
    }
}
