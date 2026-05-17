using Inventra.Application.Items.Queries.Dto;
using Inventra.Domain.Enums;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryItemQueries
{
    public async Task<InventoryStatisticsDto?> GetStatisticsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        if (!await InventoryExistsAsync(inventoryId, cancellationToken))
            return null;

        return await BuildStatisticsAsync(inventoryId, cancellationToken);
    }

    private async Task<bool> InventoryExistsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var inventories = dbContext.Inventories.AsNoTracking();

        return await inventories.AnyAsync(x => x.Id == inventoryId, cancellationToken);
    }

    private async Task<InventoryStatisticsDto> BuildStatisticsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var itemsCount = await CountItemsAsync(inventoryId, cancellationToken);
        var numericFields = await GetNumericStatisticsAsync(inventoryId, cancellationToken);
        var stringFields = await GetStringStatisticsAsync(inventoryId, cancellationToken);

        return new InventoryStatisticsDto(inventoryId, itemsCount, numericFields, stringFields);
    }

    private async Task<int> CountItemsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var items = dbContext.InventoryItems.AsNoTracking();

        return await items.CountAsync(x => x.InventoryId == inventoryId, cancellationToken);
    }

    private async Task<IReadOnlyCollection<NumericFieldStatisticsDto>> GetNumericStatisticsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await NumericStatisticsQuery(inventoryId).ToArrayAsync(cancellationToken);
    }

    private IQueryable<NumericFieldStatisticsDto> NumericStatisticsQuery(Guid inventoryId)
    {
        var fields = dbContext.InventoryFields.AsNoTracking();
        var values = dbContext.ItemFieldValues.AsNoTracking();

        return
            from field in fields
            join value in values on field.Id equals value.FieldId
            where field.InventoryId == inventoryId
            where field.Type == InventoryFieldType.Number
            where value.NumberValue != null
            group value by new { field.Id, field.Title } into fieldGroup
            orderby fieldGroup.Key.Title
            select new NumericFieldStatisticsDto(
                fieldGroup.Key.Id,
                fieldGroup.Key.Title,
                fieldGroup.Min(x => x.NumberValue),
                fieldGroup.Max(x => x.NumberValue),
                fieldGroup.Average(x => x.NumberValue));
    }

    private async Task<IReadOnlyCollection<StringFieldStatisticsDto>> GetStringStatisticsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var frequencies = await GetStringFrequenciesAsync(inventoryId, cancellationToken);

        return ToStringStatistics(frequencies);
    }

    private static StringFieldStatisticsDto[] ToStringStatistics(
        IEnumerable<StringFrequencyRow> frequencies)
    {
        return frequencies
            .GroupBy(x => new { x.FieldId, x.FieldTitle })
            .Select(group => new StringFieldStatisticsDto(
                group.Key.FieldId,
                group.Key.FieldTitle,
                MostFrequentValues(group)))
            .OrderBy(statistics => statistics.FieldTitle)
            .ToArray();
    }

    private static StringFieldFrequencyDto[] MostFrequentValues(
        IEnumerable<StringFrequencyRow> values)
    {
        return values
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Value)
            .Take(5)
            .Select(x => new StringFieldFrequencyDto(x.Value, x.Count))
            .ToArray();
    }

    private async Task<IReadOnlyCollection<StringFrequencyRow>> GetStringFrequenciesAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await StringFrequenciesQuery(inventoryId).ToArrayAsync(cancellationToken);
    }

    private IQueryable<StringFrequencyRow> StringFrequenciesQuery(Guid inventoryId)
    {
        var fields = dbContext.InventoryFields.AsNoTracking();
        var values = dbContext.ItemFieldValues.AsNoTracking();

        return
            from field in fields
            join value in values on field.Id equals value.FieldId
            where field.InventoryId == inventoryId
            where field.Type == InventoryFieldType.SingleLineText
                || field.Type == InventoryFieldType.MultiLineText
                || field.Type == InventoryFieldType.Link
            where value.TextValue != null
            group value by new { field.Id, field.Title, value.TextValue } into valueGroup
            select new StringFrequencyRow
            {
                FieldId = valueGroup.Key.Id,
                FieldTitle = valueGroup.Key.Title,
                Value = valueGroup.Key.TextValue!,
                Count = valueGroup.Count()
            };
    }
}
