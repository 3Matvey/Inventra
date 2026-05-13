using Inventra.Application.Items.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryItemQueries
{
    public async Task<InventoryItemDetailsDto?> GetDetailsAsync(
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var item = await GetItemRowAsync(itemId, cancellationToken);

        return item is null
            ? null
            : await ToDetailsAsync(item, cancellationToken);
    }

    private async Task<ItemRowBase?> GetItemRowAsync(
        Guid itemId,
        CancellationToken cancellationToken)
    {
        return await BaseItemRows()
            .Where(x => x.Id == itemId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<InventoryItemDetailsDto> ToDetailsAsync(
        ItemRowBase item,
        CancellationToken cancellationToken)
    {
        var fieldValues = await GetFieldValuesAsync([item.Id], onlyTableFields: false, cancellationToken);

        return ToDetails(item, fieldValues.Select(x => x.Value).ToArray());
    }

    private static InventoryItemDetailsDto ToDetails(
        ItemRowBase item,
        IReadOnlyCollection<ItemFieldValueDto> fieldValues)
    {
        return new InventoryItemDetailsDto(
            item.Id,
            item.InventoryId,
            item.CustomId,
            item.SequenceNumber,
            item.CreatedById,
            item.CreatedByUserName,
            item.CreatedAt,
            item.UpdatedAt,
            item.Version,
            item.LikesCount,
            item.IsLikedByCurrentUser,
            fieldValues);
    }
}
