using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Application.Inventories.Queries;

/// <summary>
/// Provides read-optimized inventory queries for table views, search, details pages, and autocomplete UI.
/// </summary>
public interface IInventoryQueries
{
    /// <summary>
    /// Gets recently created inventories for the main page latest-inventories table.
    /// </summary>
    Task<PagedResult<InventoryTableRowDto>> GetLatestAsync(
        PageRequest page,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most popular inventories ordered by item count for the main page top-inventories table.
    /// </summary>
    Task<IReadOnlyCollection<InventoryTableRowDto>> GetTopByItemsAsync(
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets full inventory page data, including settings, fields, custom ID format, tags, and access users.
    /// </summary>
    Task<InventoryDetailsDto?> GetDetailsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches inventories for the global header search and tag/category result pages.
    /// </summary>
    Task<PagedResult<InventoryTableRowDto>> SearchAsync(
        InventorySearchQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventories owned by a user for the personal page owned-inventories table.
    /// </summary>
    Task<PagedResult<InventoryTableRowDto>> GetOwnedByUserAsync(
        Guid ownerId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventories where the user has item write access, excluding ownership-only management views.
    /// </summary>
    Task<PagedResult<InventoryTableRowDto>> GetWritableByUserAsync(
        Guid userId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tag cloud data ordered by inventory usage count.
    /// </summary>
    Task<IReadOnlyCollection<TagCloudItemDto>> GetTagCloudAsync(
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets existing tags that start with the provided term for inventory tag input autocomplete.
    /// </summary>
    Task<IReadOnlyCollection<AutocompleteOptionDto>> AutocompleteTagsAsync(
        string term,
        int limit,
        CancellationToken cancellationToken = default);
}
