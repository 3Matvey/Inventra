namespace Inventra.Application.Inventories.UpdateInventoryIdFormatElement;

public sealed record UpdateInventoryIdFormatElementRequest(
    Guid InventoryId,
    Guid ElementId,
    string? Value,
    string? Format);
