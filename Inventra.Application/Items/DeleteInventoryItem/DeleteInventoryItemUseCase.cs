using Inventra.Application.Inventories;

namespace Inventra.Application.Items.DeleteInventoryItem;

public sealed class DeleteInventoryItemUseCase(
    ICurrentUser currentUser,
    IInventoryRepository inventoryRepository,
    IInventoryItemRepository itemRepository,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        DeleteInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            return ItemErrors.NotFound(request.ItemId);

        var inventory = await inventoryRepository.GetByIdAsync(item.InventoryId, cancellationToken);

        if (inventory is null)
            return ItemErrors.InventoryNotFound(item.InventoryId);

        return await DeleteAsync(request, inventory, item, cancellationToken);
    }

    private async Task<Result> DeleteAsync(
        DeleteInventoryItemRequest request,
        Inventory inventory,
        InventoryItem item,
        CancellationToken cancellationToken)
    {
        if (!InventoryPermissions.CanWriteItems(currentUser, inventory))
            return ItemErrors.AccessDenied();

        var versionResult = ItemConcurrency.EnsureExpectedVersion(item, request.ExpectedVersion);

        if (!versionResult.IsSuccess)
            return versionResult;

        itemRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
