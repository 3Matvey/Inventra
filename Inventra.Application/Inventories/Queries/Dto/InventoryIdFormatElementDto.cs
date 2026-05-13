using Inventra.Domain.Enums;

namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents one element of an inventory custom item ID format.
/// </summary>
public sealed record InventoryIdFormatElementDto(
    Guid Id,
    InventoryIdElementType Type,
    string? Value,
    string? Format,
    int Order);
