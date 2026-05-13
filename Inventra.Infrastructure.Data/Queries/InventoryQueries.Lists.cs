using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryQueries
{
    public async Task<PagedResult<InventoryTableRowDto>> GetLatestAsync(
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        var query = BaseInventoryRows().OrderByDescending(x => x.CreatedAt);

        return await ToInventoryRowsPageAsync(query, page, cancellationToken);
    }

    public async Task<IReadOnlyCollection<InventoryTableRowDto>> GetTopByItemsAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        var rowBases = await TopInventoryRowsQuery(count).ToArrayAsync(cancellationToken);

        return await AddTagsAsync(rowBases, cancellationToken);
    }

    public async Task<PagedResult<InventoryTableRowDto>> SearchAsync(
        InventorySearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var rows = SearchRowsQuery(query);

        return await ToInventoryRowsPageAsync(rows, query.Page, cancellationToken);
    }

    public async Task<PagedResult<InventoryTableRowDto>> GetOwnedByUserAsync(
        Guid ownerId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        var query = BaseInventoryRows()
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.Title);

        return await ToInventoryRowsPageAsync(query, page, cancellationToken);
    }

    public async Task<PagedResult<InventoryTableRowDto>> GetWritableByUserAsync(
        Guid userId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        var query = WritableRowsQuery(userId);

        return await ToInventoryRowsPageAsync(query, page, cancellationToken);
    }

    private IQueryable<InventoryRowBase> TopInventoryRowsQuery(int count)
    {
        return BaseInventoryRows()
            .OrderByDescending(x => x.ItemsCount)
            .ThenBy(x => x.Title)
            .Take(Math.Clamp(count, 1, 100));
    }

    private IQueryable<InventoryRowBase> SearchRowsQuery(InventorySearchQuery query)
    {
        var rows = ApplySearch(BaseInventoryRows(), query);

        return ApplySearchSorting(rows, query.Term);
    }

    private IQueryable<InventoryRowBase> WritableRowsQuery(Guid userId)
    {
        var inventoryIds = WritableInventoryIdsQuery(userId);

        return BaseInventoryRows()
            .Where(x => x.OwnerId != userId && inventoryIds.Contains(x.Id))
            .OrderBy(x => x.Title);
    }

    private IQueryable<Guid> WritableInventoryIdsQuery(Guid userId)
    {
        return dbContext.InventoryAccessGrants
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.InventoryId);
    }

    private IQueryable<InventoryRowBase> BaseInventoryRows()
    {
        return
            from inventory in dbContext.Inventories.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking() on inventory.CategoryId equals category.Id
            join owner in dbContext.UserAccounts.AsNoTracking() on inventory.OwnerId equals owner.Id
            select new InventoryRowBase
            {
                Id = inventory.Id,
                Title = inventory.Title,
                DescriptionMarkdown = inventory.DescriptionMarkdown,
                ImageUrl = inventory.ImageUrl,
                CategoryId = inventory.CategoryId,
                CategoryName = category.Name,
                OwnerId = owner.Id,
                OwnerName = owner.UserName,
                ItemsCount = dbContext.InventoryItems.Count(x => x.InventoryId == inventory.Id),
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt
            };
    }

    private IQueryable<InventoryRowBase> ApplySearch(
        IQueryable<InventoryRowBase> rows,
        InventorySearchQuery query)
    {
        rows = ApplyTermSearch(rows, query.Term);
        rows = ApplyCategoryFilter(rows, query.CategoryId);

        return ApplyTagFilter(rows, query.TagId);
    }

    private static IQueryable<InventoryRowBase> ApplyTermSearch(
        IQueryable<InventoryRowBase> rows,
        string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return rows;

        var tsQuery = EF.Functions.PlainToTsQuery("simple", term.Trim());

        return rows.Where(x =>
            EF.Functions.ToTsVector(
                "simple",
                x.Title + " " + (x.DescriptionMarkdown ?? string.Empty))
            .Matches(tsQuery));
    }

    private static IQueryable<InventoryRowBase> ApplySearchSorting(
        IQueryable<InventoryRowBase> rows,
        string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return rows.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt);

        var tsQuery = EF.Functions.PlainToTsQuery("simple", term.Trim());

        return rows
            .OrderByDescending(x => EF.Functions.ToTsVector(
                    "simple",
                    x.Title + " " + (x.DescriptionMarkdown ?? string.Empty))
                .Rank(tsQuery))
            .ThenByDescending(x => x.UpdatedAt ?? x.CreatedAt);
    }

    private static IQueryable<InventoryRowBase> ApplyCategoryFilter(
        IQueryable<InventoryRowBase> rows,
        Guid? categoryId)
    {
        return categoryId is null ? rows : rows.Where(x => x.CategoryId == categoryId.Value);
    }

    private IQueryable<InventoryRowBase> ApplyTagFilter(
        IQueryable<InventoryRowBase> rows,
        Guid? tagId)
    {
        return tagId is null
            ? rows
            : rows.Where(x => dbContext.InventoryTags.Any(tag =>
                tag.InventoryId == x.Id && tag.TagId == tagId.Value));
    }

    private async Task<PagedResult<InventoryTableRowDto>> ToInventoryRowsPageAsync(
        IQueryable<InventoryRowBase> query,
        PageRequest page,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, page.Page);
        var pageSize = Math.Clamp(page.PageSize, 1, 100);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await GetPageRowsAsync(query, pageNumber, pageSize, cancellationToken);

        return new PagedResult<InventoryTableRowDto>(items, pageNumber, pageSize, totalCount);
    }

    private async Task<IReadOnlyCollection<InventoryTableRowDto>> GetPageRowsAsync(
        IQueryable<InventoryRowBase> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var rowBases = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return await AddTagsAsync(rowBases, cancellationToken);
    }

    private async Task<IReadOnlyCollection<InventoryTableRowDto>> AddTagsAsync(
        IReadOnlyCollection<InventoryRowBase> rowBases,
        CancellationToken cancellationToken)
    {
        var tags = await GetTagRowsAsync(rowBases, cancellationToken);
        var tagsByInventoryId = GroupTagsByInventoryId(tags);

        return rowBases.Select(x => ToTableRow(x, tagsByInventoryId.GetValueOrDefault(x.Id, []))).ToArray();
    }

    private static Dictionary<Guid, TagDto[]> GroupTagsByInventoryId(
        IEnumerable<InventoryTagRow> tags)
    {
        return tags
            .GroupBy(x => x.InventoryId)
            .ToDictionary(x => x.Key, x => x.Select(tag => tag.Tag).ToArray());
    }

    private static InventoryTableRowDto ToTableRow(
        InventoryRowBase row,
        IReadOnlyCollection<TagDto> tags)
    {
        return new InventoryTableRowDto(
            row.Id,
            row.Title,
            row.DescriptionMarkdown,
            row.ImageUrl,
            row.CategoryName,
            row.OwnerId,
            row.OwnerName,
            row.ItemsCount,
            tags,
            row.CreatedAt,
            row.UpdatedAt);
    }
}
