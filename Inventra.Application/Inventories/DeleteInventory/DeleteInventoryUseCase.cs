namespace Inventra.Application.Inventories.DeleteInventory;

public sealed class DeleteInventoryUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        DeleteInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await DeleteAsync(inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> DeleteAsync(
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        inventoryRepository.Remove(inventory);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
