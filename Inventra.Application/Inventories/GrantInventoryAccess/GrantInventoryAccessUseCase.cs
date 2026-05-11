using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.GrantInventoryAccess;

public sealed class GrantInventoryAccessUseCase(
    IInventoryRepository inventoryRepository,
    IUserRepository userRepository,
    IInventoryPermissionService permissionService,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> ExecuteAsync(
        GrantInventoryAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryResult = await InventoryAccessLoader.LoadManageableAsync(
            inventoryRepository,
            permissionService,
            request.InventoryId,
            cancellationToken);

        return inventoryResult.IsSuccess
            ? await GrantAsync(request, inventoryResult.Value, cancellationToken)
            : inventoryResult.Error;
    }

    private async Task<Result<Guid>> GrantAsync(
        GrantInventoryAccessRequest request,
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail, cancellationToken);

        if (user is null)
            return InventoryErrors.AccessUserNotFound(request.UserNameOrEmail);

        return await GrantAsync(inventory, user.Id, cancellationToken);
    }

    private async Task<Result<Guid>> GrantAsync(
        Inventory inventory,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == inventory.OwnerId)
            return InventoryErrors.OwnerAccessGrantNotAllowed();

        var grant = inventory.GrantAccess(userId, timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return grant.Id;
    }
}
