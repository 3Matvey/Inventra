using Inventra.Domain.Exceptions;

namespace Inventra.Application.Inventories.UpdateInventoryIdFormatElement;

public sealed class UpdateInventoryIdFormatElementUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        UpdateInventoryIdFormatElementRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await UpdateElementAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> UpdateElementAsync(
        UpdateInventoryIdFormatElementRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (inventory.IdFormatElements.All(x => x.Id != request.ElementId))
            return InventoryErrors.IdFormatElementNotFound(request.ElementId);

        return await SaveUpdatedElementAsync(request, inventory, cancellationToken);
    }

    private async Task<Result> SaveUpdatedElementAsync(
        UpdateInventoryIdFormatElementRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        try
        {
            inventory.UpdateIdFormatElement(
                request.ElementId,
                request.Value,
                request.Format);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidInventoryIdFormatException exception)
        {
            return InventoryErrors.InvalidIdFormat(exception.Message);
        }
    }
}
