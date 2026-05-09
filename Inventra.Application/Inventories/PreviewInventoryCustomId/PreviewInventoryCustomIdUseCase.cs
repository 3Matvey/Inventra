using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Inventories.CustomIds;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.PreviewInventoryCustomId;

public sealed class PreviewInventoryCustomIdUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<Result<string>> ExecuteAsync(
        PreviewInventoryCustomIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? Preview(inventoryResult.Value)
            : inventoryResult.Error;
    }

    private string Preview(Inventory inventory)
    {
        var context = InventoryCustomIdComposer.PreviewContext(dateTimeProvider.UtcNow);

        return InventoryCustomIdComposer.Compose(inventory.IdFormatElements, context);
    }
}
