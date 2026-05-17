using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Items.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryItemQueries
{
    public async Task<PagedResult<InventoryItemTableRowDto>> GetTableAsync(
        InventoryItemsTableQuery query,
        CancellationToken cancellationToken = default)
    {
        var rows = ApplySorting(BaseItemRows(query.InventoryId), query);
        var pageNumber = Math.Max(1, query.Page.Page);
        var pageSize = Math.Clamp(query.Page.PageSize, 1, 100);
        var totalCount = await rows.CountAsync(cancellationToken);
        var items = await GetTablePageAsync(rows, pageNumber, pageSize, cancellationToken);

        return new PagedResult<InventoryItemTableRowDto>(items, pageNumber, pageSize, totalCount);
    }

    private async Task<IReadOnlyCollection<InventoryItemTableRowDto>> GetTablePageAsync(
        IQueryable<ItemRowBase> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var rowBases = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return await AddFieldValuesAsync(rowBases, onlyTableFields: true, cancellationToken);
    }

    private IQueryable<ItemRowBase> BaseItemRows(Guid? inventoryId = null)
    {
        var currentUserId = currentUser.UserId;

        return BaseItemRowsQuery(inventoryId, currentUserId);
    }

    private IQueryable<ItemRowBase> BaseItemRowsQuery(Guid? inventoryId, Guid? currentUserId)
    {
        var items = dbContext.InventoryItems.AsNoTracking();
        var creators = dbContext.UserAccounts.AsNoTracking();

        return
            from item in items
            join creator in creators on item.CreatedById equals creator.Id
            where inventoryId == null || item.InventoryId == inventoryId
            select new ItemRowBase
            {
                Id = item.Id,
                InventoryId = item.InventoryId,
                CustomId = item.CustomId,
                SequenceNumber = item.SequenceNumber,
                CreatedById = item.CreatedById,
                CreatedByUserName = creator.UserName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Version = item.Version,
                LikesCount = dbContext.ItemLikes.Count(x => x.ItemId == item.Id),
                IsLikedByCurrentUser = currentUserId != null && dbContext.ItemLikes
                    .Any(x => x.ItemId == item.Id && x.UserId == currentUserId.Value)
            };
    }

    private static IQueryable<ItemRowBase> ApplySorting(
        IQueryable<ItemRowBase> rows,
        InventoryItemsTableQuery query)
    {
        return query.SortBy?.Trim().ToLowerInvariant() switch
        {
            "created_at" => SortByCreatedAt(rows, query.SortDescending),
            "likes_count" => SortByLikesCount(rows, query.SortDescending),
            _ => SortByCustomId(rows, query.SortDescending)
        };
    }

    private static IOrderedQueryable<ItemRowBase> SortByCreatedAt(
        IQueryable<ItemRowBase> rows,
        bool descending)
    {
        return descending ? rows.OrderByDescending(x => x.CreatedAt) : rows.OrderBy(x => x.CreatedAt);
    }

    private static IOrderedQueryable<ItemRowBase> SortByLikesCount(
        IQueryable<ItemRowBase> rows,
        bool descending)
    {
        return descending ? rows.OrderByDescending(x => x.LikesCount) : rows.OrderBy(x => x.LikesCount);
    }

    private static IOrderedQueryable<ItemRowBase> SortByCustomId(
        IQueryable<ItemRowBase> rows,
        bool descending)
    {
        return descending ? rows.OrderByDescending(x => x.CustomId) : rows.OrderBy(x => x.CustomId);
    }

    private async Task<IReadOnlyCollection<InventoryItemTableRowDto>> AddFieldValuesAsync(
        IReadOnlyCollection<ItemRowBase> rowBases,
        bool onlyTableFields,
        CancellationToken cancellationToken)
    {
        var fieldValues = await GetFieldValuesForRowsAsync(rowBases, onlyTableFields, cancellationToken);
        var valuesByItemId = GroupValuesByItemId(fieldValues);

        return rowBases.Select(x => ToTableRow(x, valuesByItemId.GetValueOrDefault(x.Id, []))).ToArray();
    }

    private async Task<IReadOnlyCollection<ItemFieldValueRow>> GetFieldValuesForRowsAsync(
        IReadOnlyCollection<ItemRowBase> rowBases,
        bool onlyTableFields,
        CancellationToken cancellationToken)
    {
        var itemIds = rowBases.Select(x => x.Id).ToArray();

        return await GetFieldValuesAsync(itemIds, onlyTableFields, cancellationToken);
    }

    private static Dictionary<Guid, ItemFieldValueDto[]> GroupValuesByItemId(
        IEnumerable<ItemFieldValueRow> fieldValues)
    {
        return fieldValues
            .GroupBy(x => x.ItemId)
            .ToDictionary(x => x.Key, x => x.Select(value => value.Value).ToArray());
    }

    private static InventoryItemTableRowDto ToTableRow(
        ItemRowBase row,
        IReadOnlyCollection<ItemFieldValueDto> fieldValues)
    {
        return new InventoryItemTableRowDto(
            row.Id,
            row.CustomId,
            row.CreatedById,
            row.CreatedByUserName,
            row.CreatedAt,
            row.Version,
            row.LikesCount,
            fieldValues);
    }

    private async Task<IReadOnlyCollection<ItemFieldValueRow>> GetFieldValuesAsync(
        IReadOnlyCollection<Guid> itemIds,
        bool onlyTableFields,
        CancellationToken cancellationToken)
    {
        return await FieldValuesQuery(itemIds, onlyTableFields)
            .ToArrayAsync(cancellationToken);
    }

    private IQueryable<ItemFieldValueRow> FieldValuesQuery(
        IReadOnlyCollection<Guid> itemIds,
        bool onlyTableFields)
    {
        var values = dbContext.ItemFieldValues.AsNoTracking();
        var fields = dbContext.InventoryFields.AsNoTracking();

        return
            from value in values
            join field in fields on value.FieldId equals field.Id
            where itemIds.Contains(value.ItemId)
            where !onlyTableFields || field.ShowInTable
            orderby field.Order
            select new ItemFieldValueRow
            {
                ItemId = value.ItemId,
                Value = new ItemFieldValueDto(
                    field.Id,
                    field.Title,
                    field.Type,
                    value.TextValue,
                    value.NumberValue,
                    value.BooleanValue)
            };
    }
}
