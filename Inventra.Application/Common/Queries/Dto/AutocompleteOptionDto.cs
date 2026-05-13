namespace Inventra.Application.Common.Queries.Dto;

/// <summary>
/// Represents one selectable option returned by an autocomplete query.
/// </summary>
public sealed record AutocompleteOptionDto(
    Guid Id,
    string Label,
    string? Description = null);
