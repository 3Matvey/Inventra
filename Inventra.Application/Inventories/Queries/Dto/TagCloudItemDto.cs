namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents one tag and its weight in the main page tag cloud.
/// </summary>
public sealed record TagCloudItemDto(
    Guid Id,
    string Name,
    int InventoriesCount);
