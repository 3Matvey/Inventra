namespace Inventra.Application.Inventories.UpdateInventoryField;

public sealed class UpdateInventoryFieldUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        UpdateInventoryFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await UpdateFieldAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> UpdateFieldAsync(
        UpdateInventoryFieldRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (inventory.Fields.All(x => x.Id != request.FieldId))
            return InventoryErrors.FieldNotFound(request.FieldId);

        inventory.UpdateField(
            request.FieldId,
            request.Title,
            request.Description,
            request.ShowInTable);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
