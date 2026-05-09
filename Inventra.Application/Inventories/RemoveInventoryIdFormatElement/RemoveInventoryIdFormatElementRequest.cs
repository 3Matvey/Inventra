namespace Inventra.Application.Inventories.RemoveInventoryIdFormatElement;

public sealed record RemoveInventoryIdFormatElementRequest(
    Guid InventoryId,
    Guid ElementId);
