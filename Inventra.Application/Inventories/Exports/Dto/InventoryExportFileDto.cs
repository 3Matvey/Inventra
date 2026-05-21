namespace Inventra.Application.Inventories.Exports.Dto;

/// <summary>
/// Represents an inventory export file ready to be returned by the API.
/// </summary>
public sealed record InventoryExportFileDto(
    string FileName,
    string ContentType,
    InventoryExportFormat Format,
    InventoryExportDto Export);
