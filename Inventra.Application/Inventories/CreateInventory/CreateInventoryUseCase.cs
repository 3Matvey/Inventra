namespace Inventra.Application.Inventories.CreateInventory;

public sealed class CreateInventoryUseCase(
    ICurrentUser currentUser,
    IInventoryRepository inventoryRepository,
    ICategoryRepository categoryRepository,
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result<Guid>> ExecuteAsync(
        CreateInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return InventoryErrors.AuthenticationRequired();

        if (!await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
            return InventoryErrors.CategoryNotFound(request.CategoryId);

        var inventory = CreateInventory(request, currentUser.UserId.Value);
        var tags = await InventoryTags.ResolveAsync(tagRepository, request.Tags, cancellationToken);

        inventory.ReplaceTags(tags.Select(x => x.Id));

        await inventoryRepository.AddAsync(inventory, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return inventory.Id;
    }

    private Inventory CreateInventory(CreateInventoryRequest request, Guid ownerId)
    {
        return new Inventory(
            ownerId,
            request.CategoryId,
            request.Title,
            request.DescriptionMarkdown,
            request.ImageUrl,
            request.ImagePublicId);
    }

}
