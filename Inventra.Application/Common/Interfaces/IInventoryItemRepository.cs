using Inventra.Domain.Entities;

namespace Inventra.Application.Common.Interfaces;

public interface IInventoryItemRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);

    void Remove(InventoryItem item);
}
