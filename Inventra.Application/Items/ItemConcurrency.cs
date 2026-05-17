namespace Inventra.Application.Items;

internal static class ItemConcurrency
{
    public static Result EnsureExpectedVersion(
        InventoryItem item,
        long expectedVersion)
    {
        return item.Version == expectedVersion
            ? Result.Success()
            : ItemErrors.ConcurrencyConflict(item.Id);
    }
}
