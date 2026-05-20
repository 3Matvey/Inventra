namespace Inventra.Application.Inventories.DeleteInventory;

public sealed record DeleteInventoryRequest(
    Guid InventoryId,
    long ExpectedVersion);
