using Inventra.Domain.Enums;

namespace Inventra.Application.Inventories.AddInventoryIdFormatElement;

public sealed record AddInventoryIdFormatElementRequest(
    Guid InventoryId,
    InventoryIdElementType Type,
    string? Value,
    string? Format);
