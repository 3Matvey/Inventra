namespace Inventra.Application.Inventories.RemoveInventoryField;

public sealed class RemoveInventoryFieldUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        RemoveInventoryFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccess.LoadWithManageAccessAsync(
            inventoryRepository,
            currentUser,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await RemoveFieldAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> RemoveFieldAsync(
        RemoveInventoryFieldRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (inventory.Fields.All(x => x.Id != request.FieldId))
            return InventoryErrors.FieldNotFound(request.FieldId);

        inventory.RemoveField(request.FieldId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
