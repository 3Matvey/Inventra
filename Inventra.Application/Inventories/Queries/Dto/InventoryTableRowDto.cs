namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents one inventory row in table-based inventory lists.
/// </summary>
public sealed record InventoryTableRowDto(
    Guid Id,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl,
    string CategoryName,
    Guid OwnerId,
    string OwnerName,
    int ItemsCount,
    IReadOnlyCollection<TagDto> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
