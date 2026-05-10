namespace Inventra.Application.Items;

public sealed record ItemFieldValueRequest(
    Guid FieldId,
    string? TextValue,
    decimal? NumberValue,
    bool? BooleanValue);
