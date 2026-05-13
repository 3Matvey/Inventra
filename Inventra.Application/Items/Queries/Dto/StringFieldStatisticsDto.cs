namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents frequent-value statistics for one string custom field.
/// </summary>
public sealed record StringFieldStatisticsDto(
    Guid FieldId,
    string FieldTitle,
    IReadOnlyCollection<StringFieldFrequencyDto> MostFrequentValues);
