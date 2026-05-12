using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;

namespace Inventra.Application.Items.LikeInventoryItem;

public sealed class LikeInventoryItemUseCase(
    ICurrentUser currentUser,
    IInventoryItemRepository itemRepository,
    IInventoryPermissionService permissionService,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        LikeInventoryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!permissionService.CanLike || currentUser.UserId is null)
            return ItemErrors.AuthenticationRequired();

        var item = await itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

        if (item is null)
            return ItemErrors.NotFound(request.ItemId);

        item.Like(currentUser.UserId.Value, timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
