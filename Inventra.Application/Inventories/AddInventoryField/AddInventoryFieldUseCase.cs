using Inventra.Domain.Exceptions;

namespace Inventra.Application.Inventories.AddInventoryField;

public sealed class AddInventoryFieldUseCase(
    IInventoryRepository inventoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result<Guid>> ExecuteAsync(
        AddInventoryFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await AddFieldAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result<Guid>> AddFieldAsync(
        AddInventoryFieldRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        try
        {
            var field = inventory.AddField(
                request.Type,
                request.Title,
                request.Description,
                request.ShowInTable);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return field.Id;
        }
        catch (InventoryFieldLimitExceededException exception)
        {
            return InventoryErrors.FieldLimitExceeded(exception.Message);
        }
    }
}
