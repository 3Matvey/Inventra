namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents a compact user reference shown in read models.
/// </summary>
public sealed record UserSummaryDto(
    Guid Id,
    string UserName);
