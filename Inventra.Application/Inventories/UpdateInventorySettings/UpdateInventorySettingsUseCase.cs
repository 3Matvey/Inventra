namespace Inventra.Application.Inventories.UpdateInventorySettings;

public sealed class UpdateInventorySettingsUseCase(
    IInventoryRepository inventoryRepository,
    ICategoryRepository categoryRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result> ExecuteAsync(
        UpdateInventorySettingsRequest request,
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
        UpdateInventorySettingsRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        if (!await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
            return InventoryErrors.CategoryNotFound(request.CategoryId);

        inventory.UpdateSettings(
            request.Title,
            request.DescriptionMarkdown,
            request.CategoryId,
            request.ImageUrl);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
