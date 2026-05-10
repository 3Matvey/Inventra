using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.Items.UpdateInventoryItem;

public sealed class UpdateInventoryItemUseCase(
    IInventoryRepository inventoryRepository,
    IInventoryItemRepository itemRepository,
    IInventoryPermissionService permissionService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(
        UpdateInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            return ItemErrors.NotFound(request.ItemId);

        var inventory = await inventoryRepository.GetByIdAsync(item.InventoryId, cancellationToken);

        if (inventory is null)
            return ItemErrors.InventoryNotFound(item.InventoryId);

        return await UpdateAsync(request, inventory, item, cancellationToken);
    }

    private async Task<Result> UpdateAsync(
        UpdateInventoryItemRequest request,
        Inventory inventory,
        InventoryItem item,
        CancellationToken cancellationToken)
    {
        if (!permissionService.CanWriteItems(inventory))
            return ItemErrors.AccessDenied();

        var result = UpdateValues(request, inventory, item);

        if (!result.IsSuccess)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private Result UpdateValues(
        UpdateInventoryItemRequest request,
        Inventory inventory,
        InventoryItem item)
    {
        try
        {
            item.ChangeCustomId(request.CustomId, dateTimeProvider.UtcNow);

            foreach (var value in request.FieldValues)
            {
                var result = SetValue(value, inventory, item);

                if (!result.IsSuccess)
                    return result;
            }

            return Result.Success();
        }
        catch (InvalidItemFieldValueException exception)
        {
            return ItemErrors.InvalidFieldValue(exception.Message);
        }
    }

    private Result SetValue(
        ItemFieldValueRequest request,
        Inventory inventory,
        InventoryItem item)
    {
        var field = inventory.Fields.SingleOrDefault(x => x.Id == request.FieldId);

        if (field is null)
            return ItemErrors.FieldNotFound(request.FieldId);

        var valueResult = ItemFieldValueMapper.Map(field, request);

        if (!valueResult.IsSuccess)
            return valueResult.Error;

        item.SetFieldValue(field, valueResult.Value, dateTimeProvider.UtcNow);

        return Result.Success();
    }
}
