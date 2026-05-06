using Inventra.Domain.Entities;

namespace Inventra.Application.Common.Interfaces;

public interface IInventoryPermissionService
{
    bool CanComment { get; }

    bool CanLike { get; }

    bool CanManageInventory(Inventory inventory);

    bool CanWriteItems(Inventory inventory);
}
