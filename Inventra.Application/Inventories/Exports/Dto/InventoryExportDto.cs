namespace Inventra.Application.Inventories.Exports.Dto;

/// <summary>
/// Represents complete inventory data prepared for file export.
/// </summary>
public sealed record InventoryExportDto(
    Guid InventoryId,
    string InventoryTitle,
    string CategoryName,
    string OwnerName,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<InventoryExportFieldDto> Fields,
    IReadOnlyCollection<InventoryExportItemDto> Items);
