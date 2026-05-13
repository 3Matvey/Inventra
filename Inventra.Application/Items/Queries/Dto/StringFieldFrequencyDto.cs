namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents the frequency of one string value in inventory statistics.
/// </summary>
public sealed record StringFieldFrequencyDto(
    string Value,
    int Count);
