using Inventra.Domain.Entities;

namespace Inventra.Application.Common.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);

    void Remove(Inventory inventory);
}
