using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Items.Queries.Dto;

namespace Inventra.Application.Items.Queries;

/// <summary>
/// Provides read-optimized item queries for inventory item tables, item details pages, and statistics tabs.
/// </summary>
public interface IInventoryItemQueries
{
    /// <summary>
    /// Gets the default table view for items inside one inventory, including fixed fields and visible custom fields.
    /// </summary>
    Task<PagedResult<InventoryItemTableRowDto>> GetTableAsync(
        InventoryItemsTableQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single item details view, including custom field values and like state for the current user when available.
    /// </summary>
    Task<InventoryItemDetailsDto?> GetDetailsAsync(
        Guid itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets aggregate inventory statistics, such as item count, numeric ranges/averages, and frequent string values.
    /// </summary>
    Task<InventoryStatisticsDto?> GetStatisticsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default);
}
