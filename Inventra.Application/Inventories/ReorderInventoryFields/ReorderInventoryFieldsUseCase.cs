using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.ReorderInventoryFields;

public sealed class ReorderInventoryFieldsUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(
        ReorderInventoryFieldsRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
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

        inventory.ReorderFields(request.OrderedFieldIds, dateTimeProvider.UtcNow);
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
