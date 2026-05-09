namespace Inventra.Application.Inventories.GrantInventoryAccess;

public sealed record GrantInventoryAccessRequest(
    Guid InventoryId,
    string UserNameOrEmail);
