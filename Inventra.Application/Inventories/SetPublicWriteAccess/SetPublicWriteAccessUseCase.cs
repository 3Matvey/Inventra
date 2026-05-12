using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.SetPublicWriteAccess;

public sealed class SetPublicWriteAccessUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        SetPublicWriteAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await SetPublicAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> SetPublicAsync(
        SetPublicWriteAccessRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        inventory.SetPublicWriteAccess(request.IsPublic);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
