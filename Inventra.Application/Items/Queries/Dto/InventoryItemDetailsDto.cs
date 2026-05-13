namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents the full read model for an inventory item details page.
/// </summary>
public sealed record InventoryItemDetailsDto(
    Guid Id,
    Guid InventoryId,
    string CustomId,
    long? SequenceNumber,
    Guid CreatedById,
    string CreatedByUserName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    long Version,
    int LikesCount,
    bool IsLikedByCurrentUser,
    IReadOnlyCollection<ItemFieldValueDto> FieldValues);
