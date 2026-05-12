using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.ReorderInventoryIdFormatElements;

public sealed class ReorderInventoryIdFormatElementsUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        ReorderInventoryIdFormatElementsRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await ReorderElementsAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> ReorderElementsAsync(
        ReorderInventoryIdFormatElementsRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (!ContainsEveryElementOnce(inventory, request.OrderedElementIds))
            return InventoryErrors.InvalidIdElementOrder();

        inventory.ReorderIdFormatElements(request.OrderedElementIds);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static bool ContainsEveryElementOnce(
        Inventory inventory,
        IReadOnlyList<Guid> orderedElementIds)
    {
        var elementIds = inventory.IdFormatElements.Select(x => x.Id).ToHashSet();

        return orderedElementIds.Count == elementIds.Count &&
            orderedElementIds.Distinct().All(elementIds.Contains);
    }
}
