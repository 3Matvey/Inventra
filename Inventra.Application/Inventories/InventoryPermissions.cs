namespace Inventra.Application.Inventories;

internal static class InventoryPermissions
{
    public static bool CanComment(ICurrentUser currentUser)
    {
        return currentUser.IsAuthenticated;
    }

    public static bool CanLike(ICurrentUser currentUser)
    {
        return currentUser.IsAuthenticated;
    }

    public static bool CanManageInventory(ICurrentUser currentUser, Inventory inventory)
    {
        return currentUser.IsAdmin || IsOwner(currentUser, inventory);
    }

    public static bool CanWriteItems(ICurrentUser currentUser, Inventory inventory)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return false;

        return currentUser.IsAdmin
            || IsOwner(currentUser, inventory)
            || inventory.IsPublicWriteAccess
            || inventory.HasExplicitWriteAccess(currentUser.UserId.Value);
    }

    private static bool IsOwner(ICurrentUser currentUser, Inventory inventory)
    {
        return currentUser.UserId == inventory.OwnerId;
    }
}
