using Inventra.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Data.Repositories;

public class InventorySequenceProvider(AppDbContext dbContext) : IInventorySequenceProvider
{
    public async Task<long> GetNextSequenceAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Database.SqlQuery<long>(
            $"""
             INSERT INTO inventory_sequences (inventory_id, next_value)
             VALUES ({inventoryId}, 2)
             ON CONFLICT (inventory_id)
             DO UPDATE SET next_value = inventory_sequences.next_value + 1
             RETURNING inventory_sequences.next_value - 1 AS "Value"
             """);

        return await query.SingleAsync(cancellationToken);
    }
}
