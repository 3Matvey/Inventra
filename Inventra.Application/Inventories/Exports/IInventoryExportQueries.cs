using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Application.Inventories.Exports;

/// <summary>
/// Provides read-optimized data for inventory file exports.
/// </summary>
public interface IInventoryExportQueries
{
    Task<InventoryExportDto?> GetAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default);
}
