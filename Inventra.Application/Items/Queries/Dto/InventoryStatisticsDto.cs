namespace Inventra.Application.Items.Queries.Dto;

/// <summary>
/// Represents aggregate statistics for one inventory.
/// </summary>
public sealed record InventoryStatisticsDto(
    Guid InventoryId,
    int ItemsCount,
    IReadOnlyCollection<NumericFieldStatisticsDto> NumericFields,
    IReadOnlyCollection<StringFieldStatisticsDto> StringFields);
