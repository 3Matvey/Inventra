namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents one item row in an inventory items table.
/// </summary>
public sealed record InventoryItemTableRowDto(
    Guid Id,
    string CustomId,
    Guid CreatedById,
    string CreatedByUserName,
    DateTimeOffset CreatedAt,
    long Version,
    int LikesCount,
    IReadOnlyCollection<ItemFieldValueDto> FieldValues);
