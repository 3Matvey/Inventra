namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents a tag shown in inventory read models.
/// </summary>
public sealed record TagDto(
    Guid Id,
    string Name);
