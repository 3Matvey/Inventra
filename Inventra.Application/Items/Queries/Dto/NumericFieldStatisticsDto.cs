namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents aggregate statistics for one numeric custom field.
/// </summary>
public sealed record NumericFieldStatisticsDto(
    Guid FieldId,
    string FieldTitle,
    decimal? Min,
    decimal? Max,
    decimal? Average);
