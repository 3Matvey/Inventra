namespace Inventra.Application.Inventories.RemoveInventoryIdFormatElement;

public sealed record RemoveInventoryIdFormatElementRequest(
    Guid InventoryId,
    long ExpectedVersion,
    Guid ElementId);
