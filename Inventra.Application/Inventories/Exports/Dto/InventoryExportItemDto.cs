namespace Inventra.Application.Inventories.Exports.Dto;

/// <summary>
/// Represents one item row in an inventory export.
/// </summary>
public sealed record InventoryExportItemDto(
    Guid ItemId,
    string CustomId,
    string CreatedByUserName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    int LikesCount,
    IReadOnlyCollection<InventoryExportValueDto> Values);
