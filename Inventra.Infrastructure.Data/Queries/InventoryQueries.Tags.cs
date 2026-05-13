using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryQueries
{
    public async Task<IReadOnlyCollection<TagCloudItemDto>> GetTagCloudAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await TagCloudQuery()
            .Take(Math.Clamp(count, 1, 100))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AutocompleteOptionDto>> AutocompleteTagsAsync(
        string term,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await TagAutocompleteQuery(term)
            .Take(Math.Clamp(limit, 1, 50))
            .ToArrayAsync(cancellationToken);
    }

    private IQueryable<TagCloudItemDto> TagCloudQuery()
    {
        return
            from inventoryTag in dbContext.InventoryTags.AsNoTracking()
            join tag in dbContext.Tags.AsNoTracking() on inventoryTag.TagId equals tag.Id
            group inventoryTag by new { tag.Id, tag.Name } into tagGroup
            orderby tagGroup.Count() descending, tagGroup.Key.Name
            select ToTagCloudItem(tagGroup.Key.Id, tagGroup.Key.Name, tagGroup.Count());
    }

    private static TagCloudItemDto ToTagCloudItem(Guid id, string name, int count)
    {
        return new TagCloudItemDto(id, name, count);
    }

    private IQueryable<AutocompleteOptionDto> TagAutocompleteQuery(string term)
    {
        var pattern = term.Trim() + "%";

        return dbContext.Tags
            .AsNoTracking()
            .Where(x => EF.Functions.ILike(x.Name, pattern))
            .OrderBy(x => x.Name)
            .Select(x => new AutocompleteOptionDto(x.Id, x.Name));
    }

    private async Task<IReadOnlyCollection<TagDto>> GetTagsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await GetTagsQuery([inventoryId])
            .Select(x => x.Tag)
            .ToArrayAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<InventoryTagRow>> GetTagRowsAsync(
        IReadOnlyCollection<InventoryRowBase> rowBases,
        CancellationToken cancellationToken)
    {
        var inventoryIds = rowBases.Select(x => x.Id).ToArray();

        return await GetTagsQuery(inventoryIds).ToArrayAsync(cancellationToken);
    }

    private IQueryable<InventoryTagRow> GetTagsQuery(IReadOnlyCollection<Guid> inventoryIds)
    {
        return
            from inventoryTag in dbContext.InventoryTags.AsNoTracking()
            join tag in dbContext.Tags.AsNoTracking() on inventoryTag.TagId equals tag.Id
            where inventoryIds.Contains(inventoryTag.InventoryId)
            orderby tag.Name
            select new InventoryTagRow
            {
                InventoryId = inventoryTag.InventoryId,
                Tag = new TagDto(tag.Id, tag.Name)
            };
    }
}
