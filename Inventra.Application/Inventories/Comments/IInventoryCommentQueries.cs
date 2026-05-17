using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Inventories.Comments.Dto;

namespace Inventra.Application.Inventories.Comments;

public interface IInventoryCommentQueries
{
    /// <summary>
    /// Gets one page of comments for an inventory discussion ordered by creation time.
    /// </summary>
    Task<PagedResult<InventoryCommentDto>?> GetPageAsync(
        Guid inventoryId,
        PageRequest page,
        CancellationToken cancellationToken = default);
}
