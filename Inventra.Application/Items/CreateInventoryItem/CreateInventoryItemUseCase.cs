using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.Items.CreateInventoryItem;

public sealed class CreateInventoryItemUseCase(
    ICurrentUser currentUser,
    IInventoryRepository inventoryRepository,
    IInventoryItemRepository itemRepository,
    IInventoryPermissionService permissionService,
    ICustomIdGenerator customIdGenerator,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> ExecuteAsync(
        CreateInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return ItemErrors.AuthenticationRequired();

        var inventory = await inventoryRepository.GetByIdAsync(request.InventoryId, cancellationToken);

        if (inventory is null)
            return ItemErrors.InventoryNotFound(request.InventoryId);

        return await CreateAsync(request, inventory, currentUser.UserId.Value, cancellationToken);
    }

    private async Task<Result<Guid>> CreateAsync(
        CreateInventoryItemRequest request,
        Inventory inventory,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!permissionService.CanWriteItems(inventory))
            return ItemErrors.AccessDenied();

        var itemResult = await CreateItemAsync(request, inventory, userId, cancellationToken);

        if (!itemResult.IsSuccess)
            return itemResult.Error;

        await itemRepository.AddAsync(itemResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return itemResult.Value.Id;
    }

    private async Task<Result<InventoryItem>> CreateItemAsync(
        CreateInventoryItemRequest request,
        Inventory inventory,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var customId = await customIdGenerator.GenerateAsync(inventory, cancellationToken);

        if (string.IsNullOrWhiteSpace(customId))
            return ItemErrors.CustomIdFormatNotConfigured();

        var item = new InventoryItem(inventory.Id, userId, customId, dateTimeProvider.UtcNow);
        var valuesResult = SetValues(request.FieldValues, inventory, item);

        if (!valuesResult.IsSuccess)
            return valuesResult.Error;

        return item;
    }

    private Result SetValues(
        IReadOnlyCollection<ItemFieldValueRequest> values,
        Inventory inventory,
        InventoryItem item)
    {
        try
        {
            foreach (var value in values)
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
