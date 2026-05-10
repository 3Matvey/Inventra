using Inventra.Application.Common.Interfaces;
using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Data.Repositories;

public class InventoryRepository(AppDbContext dbContext) : IInventoryRepository
{
    public Task<Inventory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Inventories
            .Include(x => x.Fields)
            .Include(x => x.AccessGrants)
            .Include(x => x.IdFormatElements)
            .Include(x => x.Tags)
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Inventory inventory,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Inventories.AddAsync(inventory, cancellationToken);
    }

    public void Remove(Inventory inventory)
    {
        dbContext.Inventories.Remove(inventory);
    }
}
