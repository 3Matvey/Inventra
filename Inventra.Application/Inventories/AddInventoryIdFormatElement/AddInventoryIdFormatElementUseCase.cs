using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.Inventories.AddInventoryIdFormatElement;

public sealed class AddInventoryIdFormatElementUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryPermissionService permissionService,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result<Guid>> ExecuteAsync(
        AddInventoryIdFormatElementRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await AddElementAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result<Guid>> AddElementAsync(
        AddInventoryIdFormatElementRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        try
        {
            var element = inventory.AddIdFormatElement(
                request.Type,
                request.Value,
                request.Format);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return element.Id;
        }
        catch (InvalidInventoryIdFormatException exception)
        {
            return InventoryErrors.InvalidIdFormat(exception.Message);
        }
    }
}
