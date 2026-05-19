namespace Inventra.Api.Controllers.Responses;

public sealed record HealthResponse(
    string Status,
    DateTimeOffset CheckedAt);
