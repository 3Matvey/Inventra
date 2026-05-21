namespace Inventra.Application.Inventories.ReorderInventoryFields;

public sealed class ReorderInventoryFieldsUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        ReorderInventoryFieldsRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await ReorderFieldsAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> ReorderFieldsAsync(
        ReorderInventoryFieldsRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (!ContainsEveryFieldOnce(inventory, request.OrderedFieldIds))
            return InventoryErrors.InvalidFieldOrder();

        InventoryOrderReorder.MoveFieldsToTemporaryOrders(inventory);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        inventory.ReorderFields(request.OrderedFieldIds);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static bool ContainsEveryFieldOnce(
        Inventory inventory,
        IReadOnlyList<Guid> orderedFieldIds)
    {
        var fieldIds = inventory.Fields.Select(x => x.Id).ToHashSet();

        return orderedFieldIds.Count == fieldIds.Count &&
            orderedFieldIds.Distinct().All(fieldIds.Contains);
    }
}
