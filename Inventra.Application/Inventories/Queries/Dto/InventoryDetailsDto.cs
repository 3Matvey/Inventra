namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents the full read model for an inventory details page.
/// </summary>
public sealed record InventoryDetailsDto(
    Guid Id,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl,
    CategoryDto Category,
    UserSummaryDto Owner,
    bool IsPublicWriteAccess,
    long Version,
    IReadOnlyCollection<TagDto> Tags,
    IReadOnlyCollection<InventoryFieldDefinitionDto> Fields,
    IReadOnlyCollection<InventoryIdFormatElementDto> IdFormatElements,
    IReadOnlyCollection<InventoryAccessUserDto> AccessUsers);
