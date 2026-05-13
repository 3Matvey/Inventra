using Inventra.Domain.Enums;

namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents one custom field value in an item read model.
/// </summary>
public sealed record ItemFieldValueDto(
    Guid FieldId,
    string FieldTitle,
    InventoryFieldType FieldType,
    string? TextValue,
    decimal? NumberValue,
    bool? BooleanValue);
