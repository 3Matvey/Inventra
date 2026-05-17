namespace Inventra.Api.Hubs;

internal static class InventoryDiscussionGroups
{
    public static string ForInventory(Guid inventoryId)
    {
        return $"inventory:{inventoryId}:discussion";
    }
}
