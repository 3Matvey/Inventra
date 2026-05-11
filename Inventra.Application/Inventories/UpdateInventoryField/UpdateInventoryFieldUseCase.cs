using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.UpdateInventoryField;

public sealed class UpdateInventoryFieldUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(
        UpdateInventoryFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
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
