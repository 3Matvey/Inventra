namespace Inventra.Application.Inventories.Exports.Dto;

/// <summary>
/// Represents one custom field value in an inventory export row.
/// </summary>
public sealed record InventoryExportValueDto(
    Guid FieldId,
    string? TextValue,
    decimal? NumberValue,
    bool? BooleanValue);
