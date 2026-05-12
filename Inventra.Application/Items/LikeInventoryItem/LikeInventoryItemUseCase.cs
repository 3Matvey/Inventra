using Inventra.Application.Inventories;

namespace Inventra.Application.Items.LikeInventoryItem;

public sealed class LikeInventoryItemUseCase(
    ICurrentUser currentUser,
    IInventoryItemRepository itemRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        LikeInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!InventoryPermissions.CanLike(currentUser) || currentUser.UserId is null)
            return ItemErrors.AuthenticationRequired();

        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            return ItemErrors.NotFound(request.ItemId);

        item.Like(currentUser.UserId.Value, timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
