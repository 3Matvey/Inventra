namespace Inventra.Application.Inventories.UpdateInventoryIdFormatElement;

public sealed record UpdateInventoryIdFormatElementRequest(
    Guid InventoryId,
    long ExpectedVersion,
    Guid ElementId,
    string? Value,
    string? Format);
