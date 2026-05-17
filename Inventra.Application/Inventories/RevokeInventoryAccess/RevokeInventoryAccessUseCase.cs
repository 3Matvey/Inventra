namespace Inventra.Application.Inventories.RevokeInventoryAccess;

public sealed class RevokeInventoryAccessUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        RevokeInventoryAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
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
