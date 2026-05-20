namespace Inventra.Application.Inventories.UpdateInventoryTags;

public sealed class UpdateInventoryTagsUseCase(
    IInventoryRepository inventoryRepository,
    ITagRepository tagRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        UpdateInventoryTagsRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await inventoryRepository.LoadWithManageAccessAndVersionAsync(currentUser,
            request.InventoryId,
            request.ExpectedVersion,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await UpdateAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result> UpdateAsync(
        UpdateInventoryTagsRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        var tags = await InventoryTags.ResolveAsync(tagRepository, request.Tags, cancellationToken);

        inventory.ReplaceTags(tags.Select(x => x.Id));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
