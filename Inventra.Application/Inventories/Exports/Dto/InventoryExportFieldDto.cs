using Inventra.Domain.Enums;

namespace Inventra.Application.Inventories.Exports.Dto;

/// <summary>
/// Represents one custom field column in an inventory export.
/// </summary>
public sealed record InventoryExportFieldDto(
    Guid FieldId,
    string Title,
    InventoryFieldType Type,
    int Order);
