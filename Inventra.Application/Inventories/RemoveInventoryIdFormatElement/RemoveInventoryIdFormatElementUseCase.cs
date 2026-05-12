using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.RemoveInventoryIdFormatElement;

public sealed class RemoveInventoryIdFormatElementUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        RemoveInventoryIdFormatElementRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await RemoveElementAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> RemoveElementAsync(
        RemoveInventoryIdFormatElementRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (inventory.IdFormatElements.All(x => x.Id != request.ElementId))
            return InventoryErrors.IdFormatElementNotFound(request.ElementId);

        inventory.RemoveIdFormatElement(request.ElementId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
