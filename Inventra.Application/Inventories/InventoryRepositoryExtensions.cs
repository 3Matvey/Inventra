namespace Inventra.Application.Inventories;

internal static class InventoryRepositoryExtensions
{
    extension(IInventoryRepository inventoryRepository)
    {
        public async Task<Result<Inventory>> LoadWithManageAccessAsync(
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

        public async Task<Result<Inventory>> LoadWithManageAccessAndVersionAsync(
            ICurrentUser currentUser,
            Guid inventoryId,
            long expectedVersion,
            CancellationToken cancellationToken = default)
        {
            var result = await inventoryRepository.LoadWithManageAccessAsync(
                currentUser,
                inventoryId,
                cancellationToken);

            return result.IsSuccess
                ? EnsureExpectedVersion(result.Value, expectedVersion)
                : result.Error;
        }
    }

    private static Result<Inventory> EnsureExpectedVersion(
        Inventory inventory,
        long expectedVersion)
    {
        return inventory.Version == expectedVersion
            ? inventory
            : InventoryErrors.ConcurrencyConflict(inventory.Id);
    }
}
