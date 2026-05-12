using Inventra.Application.Inventories.CustomIds;

namespace Inventra.Application.Inventories.PreviewInventoryCustomId;

public sealed class PreviewInventoryCustomIdUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    TimeProvider timeProvider)
    : IUseCase
{
    public async Task<Result<string>> ExecuteAsync(
        PreviewInventoryCustomIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccess.LoadWithManageAccessAsync(
            inventoryRepository,
            currentUser,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? Preview(inventoryResult.Value)
            : inventoryResult.Error;
    }

    private string Preview(Inventory inventory)
    {
        var context = InventoryCustomIdComposer.PreviewContext(timeProvider.GetUtcNow());

        return InventoryCustomIdComposer.Compose(inventory.IdFormatElements, context);
    }
}
