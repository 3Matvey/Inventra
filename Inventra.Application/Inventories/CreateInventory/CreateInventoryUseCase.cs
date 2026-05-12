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
        var tags = await ResolveTagsAsync(request.Tags, cancellationToken);

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
            request.ImageUrl);
    }

    private async Task<IReadOnlyCollection<Tag>> ResolveTagsAsync(
        IReadOnlyCollection<string> tagNames,
        CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        foreach (var tagName in NormalizeTags(tagNames))
            tags.Add(await GetOrCreateTagAsync(tagName, cancellationToken));

        return tags;
    }

    private async Task<Tag> GetOrCreateTagAsync(
        string tagName,
        CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByNameAsync(tagName, cancellationToken);

        if (tag is not null)
            return tag;

        tag = new Tag(tagName);
        await tagRepository.AddAsync(tag, cancellationToken);

        return tag;
    }

    private static string[] NormalizeTags(IEnumerable<string> tagNames)
    {
        return [.. tagNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }
}
