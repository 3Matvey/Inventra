namespace Inventra.Application.Inventories;

internal static class InventoryAccess
{
    public static async Task<Result<Inventory>> LoadWithManageAccessAsync(
        IInventoryRepository inventoryRepository,
        ICurrentUser currentUser,
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);

        if (inventory is null)
            return InventoryErrors.NotFound(inventoryId);

        if (!InventoryPermissions.CanManageInventory(currentUser, inventory))
            return InventoryErrors.AccessDenied();

        return inventory;
    }
}
