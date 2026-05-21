using Inventra.Application.Inventories.Exports;
using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal sealed class InventoryExportQueries(AppDbContext dbContext) : IInventoryExportQueries
{
    public async Task<InventoryExportDto?> GetAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetInventoryAsync(inventoryId, cancellationToken);

        return inventory is null
            ? null
            : await FillInventoryAsync(inventory, inventoryId, cancellationToken);
    }

    private async Task<InventoryExportDto> FillInventoryAsync(
        InventoryExportDto inventory,
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var fields = await GetFieldsAsync(inventoryId, cancellationToken);
        var items = await GetItemsAsync(inventoryId, fields, cancellationToken);

        return inventory with { Fields = fields, Items = items };
    }

    private async Task<InventoryExportDto?> GetInventoryAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await InventoryQuery(inventoryId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<InventoryExportDto> InventoryQuery(Guid inventoryId)
    {
        var inventories = dbContext.Inventories.AsNoTracking();
        var categories = dbContext.Categories.AsNoTracking();
        var owners = dbContext.UserAccounts.AsNoTracking();

        return
            from inventory in inventories
            join category in categories on inventory.CategoryId equals category.Id
            join owner in owners on inventory.OwnerId equals owner.Id
            where inventory.Id == inventoryId
            select ToExportBase(inventory, category, owner);
    }

    private static InventoryExportDto ToExportBase(
        Inventory inventory,
        Category category,
        UserAccount owner)
    {
        return new InventoryExportDto(
            inventory.Id,
            inventory.Title,
            category.Name,
            owner.UserName,
            inventory.CreatedAt,
            Array.Empty<InventoryExportFieldDto>(),
            Array.Empty<InventoryExportItemDto>());
    }

    private async Task<IReadOnlyCollection<InventoryExportFieldDto>> GetFieldsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var fields = dbContext.InventoryFields.AsNoTracking();

        return await fields
            .Where(x => x.InventoryId == inventoryId)
            .OrderBy(x => x.Order)
            .Select(x => new InventoryExportFieldDto(x.Id, x.Title, x.Type, x.Order))
            .ToArrayAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<InventoryExportItemDto>> GetItemsAsync(
        Guid inventoryId,
        IReadOnlyCollection<InventoryExportFieldDto> fields,
        CancellationToken cancellationToken)
    {
        var items = await ItemRowsQuery(inventoryId).ToArrayAsync(cancellationToken);
        var values = await GetValuesAsync(items.Select(x => x.ItemId).ToArray(), cancellationToken);
        var valuesByItemId = GroupValuesByItemId(values);

        return items.Select(x => ToItem(x, fields, valuesByItemId)).ToArray();
    }

    private IQueryable<InventoryExportItemRow> ItemRowsQuery(Guid inventoryId)
    {
        var items = dbContext.InventoryItems.AsNoTracking();
        var creators = dbContext.UserAccounts.AsNoTracking();

        return
            from item in items
            join creator in creators on item.CreatedById equals creator.Id
            where item.InventoryId == inventoryId
            orderby item.CustomId
            select new InventoryExportItemRow
            {
                ItemId = item.Id,
                CustomId = item.CustomId,
                CreatedByUserName = creator.UserName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                LikesCount = dbContext.ItemLikes.Count(x => x.ItemId == item.Id)
            };
    }

    private async Task<IReadOnlyCollection<InventoryExportValueRow>> GetValuesAsync(
        IReadOnlyCollection<Guid> itemIds,
        CancellationToken cancellationToken)
    {
        return itemIds.Count == 0
            ? []
            : await ValuesQuery(itemIds).ToArrayAsync(cancellationToken);
    }

    private IQueryable<InventoryExportValueRow> ValuesQuery(IReadOnlyCollection<Guid> itemIds)
    {
        var values = dbContext.ItemFieldValues.AsNoTracking();

        return values
            .Where(x => itemIds.Contains(x.ItemId))
            .Select(x => new InventoryExportValueRow
            {
                ItemId = x.ItemId,
                Value = new InventoryExportValueDto(
                    x.FieldId,
                    x.TextValue,
                    x.NumberValue,
                    x.BooleanValue)
            });
    }

    private static Dictionary<Guid, InventoryExportValueDto[]> GroupValuesByItemId(
        IEnumerable<InventoryExportValueRow> rows)
    {
        return rows
            .GroupBy(x => x.ItemId)
            .ToDictionary(x => x.Key, x => x.Select(value => value.Value).ToArray());
    }

    private static InventoryExportItemDto ToItem(
        InventoryExportItemRow row,
        IReadOnlyCollection<InventoryExportFieldDto> fields,
        IReadOnlyDictionary<Guid, InventoryExportValueDto[]> valuesByItemId)
    {
        var values = valuesByItemId.GetValueOrDefault(row.ItemId, []);

        return new InventoryExportItemDto(
            row.ItemId,
            row.CustomId,
            row.CreatedByUserName,
            row.CreatedAt,
            row.UpdatedAt,
            row.LikesCount,
            OrderValues(fields, values));
    }

    private static IReadOnlyCollection<InventoryExportValueDto> OrderValues(
        IEnumerable<InventoryExportFieldDto> fields,
        IEnumerable<InventoryExportValueDto> values)
    {
        var valuesByFieldId = values.ToDictionary(x => x.FieldId);

        return fields
            .Select(field => valuesByFieldId.GetValueOrDefault(field.FieldId))
            .Where(value => value is not null)
            .Select(value => value!)
            .ToArray();
    }

    private sealed class InventoryExportItemRow
    {
        public Guid ItemId { get; init; }
        public string CustomId { get; init; } = string.Empty;
        public string CreatedByUserName { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public int LikesCount { get; init; }
    }

    private sealed class InventoryExportValueRow
    {
        public Guid ItemId { get; init; }
        public InventoryExportValueDto Value { get; init; } = null!;
    }
}
