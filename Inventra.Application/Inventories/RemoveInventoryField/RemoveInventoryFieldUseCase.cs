using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.RemoveInventoryField;

public sealed class RemoveInventoryFieldUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(
        RemoveInventoryFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
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

        inventory.RemoveField(request.FieldId, dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
