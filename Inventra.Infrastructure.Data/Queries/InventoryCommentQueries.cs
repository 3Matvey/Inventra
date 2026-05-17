using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Inventories.Comments;
using Inventra.Application.Inventories.Comments.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal class InventoryCommentQueries(AppDbContext dbContext) : IInventoryCommentQueries
{
    public async Task<PagedResult<InventoryCommentDto>?> GetPageAsync(
        Guid inventoryId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        if (!await InventoryExistsAsync(inventoryId, cancellationToken))
            return null;

        var query = CommentRows(inventoryId);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await PageRows(query, page).ToArrayAsync(cancellationToken);

        return new PagedResult<InventoryCommentDto>(items, page.Page, page.PageSize, totalCount);
    }

    private Task<bool> InventoryExistsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var inventories = dbContext.Inventories.AsNoTracking();

        return inventories.AnyAsync(x => x.Id == inventoryId, cancellationToken);
    }

    private IQueryable<InventoryCommentDto> CommentRows(Guid inventoryId)
    {
        var comments = dbContext.InventoryComments.AsNoTracking();
        var authors = dbContext.UserAccounts.AsNoTracking();

        return from comment in comments
               join author in authors
                 on comment.AuthorId equals author.Id
               where comment.InventoryId == inventoryId
               orderby comment.CreatedAt, comment.Id
               select new InventoryCommentDto(
                    comment.Id,
                    comment.InventoryId,
                    comment.AuthorId,
                    author.UserName,
                    comment.BodyMarkdown,
                    comment.CreatedAt);
    }

    private static IQueryable<InventoryCommentDto> PageRows(
        IQueryable<InventoryCommentDto> query,
        PageRequest page)
    {
        return query
            .Skip((page.Page - 1) * page.PageSize)
            .Take(page.PageSize);
    }
}
