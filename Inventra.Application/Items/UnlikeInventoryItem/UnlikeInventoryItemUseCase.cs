using Inventra.Application.Inventories;

namespace Inventra.Application.Items.UnlikeInventoryItem;

public sealed class UnlikeInventoryItemUseCase(
    ICurrentUser currentUser,
    IInventoryItemRepository itemRepository,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        UnlikeInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!InventoryPermissions.CanLike(currentUser) || currentUser.UserId is null)
            return ItemErrors.AuthenticationRequired();

        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            return ItemErrors.NotFound(request.ItemId);

        item.Unlike(currentUser.UserId.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
