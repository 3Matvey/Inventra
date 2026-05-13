using Inventra.Domain.Enums;

namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents a custom field definition displayed or edited on an inventory page.
/// </summary>
public sealed record InventoryFieldDefinitionDto(
    Guid Id,
    InventoryFieldType Type,
    string Title,
    string? Description,
    bool ShowInTable,
    int Order);
