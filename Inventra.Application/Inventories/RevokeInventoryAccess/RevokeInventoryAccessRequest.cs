namespace Inventra.Application.Inventories.RevokeInventoryAccess;

public sealed record RevokeInventoryAccessRequest(
    Guid InventoryId,
    Guid UserId);
