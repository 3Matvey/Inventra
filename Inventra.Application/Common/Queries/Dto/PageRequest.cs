namespace Inventra.Application.Common.Queries.Dto;

/// <summary>
/// Represents pagination parameters for read-side table queries.
/// </summary>
public sealed record PageRequest(int Page = 1, int PageSize = 50);
