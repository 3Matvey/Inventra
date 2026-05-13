namespace Inventra.Application.Common.Queries.Dto;

/// <summary>
/// Represents one page of read-side results with total item count.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount);
