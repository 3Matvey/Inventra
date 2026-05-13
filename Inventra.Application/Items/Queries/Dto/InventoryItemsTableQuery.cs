using Inventra.Application.Common.Queries.Dto;

namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents filters, paging, and sorting for an inventory items table.
/// </summary>
public sealed record InventoryItemsTableQuery(
    Guid InventoryId,
    PageRequest Page,
    string? SortBy = null,
    bool SortDescending = false);
