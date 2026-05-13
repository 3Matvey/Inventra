namespace Inventra.Infrastructure.Data.Repositories;

internal class InventoryItemRepository(AppDbContext dbContext) : IInventoryItemRepository
{
    public Task<InventoryItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.InventoryItems
            .Include(x => x.FieldValues)
            .Include(x => x.Likes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default)
    {
        await dbContext.InventoryItems.AddAsync(item, cancellationToken);
    }

    public void Remove(InventoryItem item)
    {
        dbContext.InventoryItems.Remove(item);
    }
}
