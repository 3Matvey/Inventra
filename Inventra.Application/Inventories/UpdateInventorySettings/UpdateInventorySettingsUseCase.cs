using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.UpdateInventorySettings;

public sealed class UpdateInventorySettingsUseCase(
    IInventoryRepository inventoryRepository,
    ICategoryRepository categoryRepository,
    IInventoryPermissionService permissionService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(
        UpdateInventorySettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
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
            request.ImageUrl,
            dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
