using Inventra.Application.Common.Queries.Dto;

namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents filters and paging for inventory search result queries.
/// </summary>
public sealed record InventorySearchQuery(
    string Term,
    PageRequest Page,
    Guid? CategoryId = null,
    Guid? TagId = null);
