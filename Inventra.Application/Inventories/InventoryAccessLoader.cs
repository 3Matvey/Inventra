using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories;

internal static class InventoryAccessLoader
{
    public static async Task<Result<Inventory>> LoadManageableAsync(
        IInventoryRepository inventoryRepository,
        IInventoryPermissionService permissionService,
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);

        if (inventory is null)
            return InventoryErrors.NotFound(inventoryId);

        if (!permissionService.CanManageInventory(inventory))
            return InventoryErrors.AccessDenied();

        return inventory;
    }
}
