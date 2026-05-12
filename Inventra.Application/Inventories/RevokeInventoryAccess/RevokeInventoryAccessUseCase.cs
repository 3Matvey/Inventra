using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.RevokeInventoryAccess;

public sealed class RevokeInventoryAccessUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        RevokeInventoryAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await RevokeAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> RevokeAsync(
        RevokeInventoryAccessRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        inventory.RevokeAccess(request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
