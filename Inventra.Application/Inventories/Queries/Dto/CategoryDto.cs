namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents a category shown in inventory read models.
/// </summary>
public sealed record CategoryDto(
    Guid Id,
    string Name);
