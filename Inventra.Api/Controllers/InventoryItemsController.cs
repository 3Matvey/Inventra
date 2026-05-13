using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Items.Queries;
using Inventra.Application.Items.Queries.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("inventories/{inventoryId:guid}/items")]
public class InventoryItemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTable(
        Guid inventoryId,
        [FromServices] IInventoryItemQueries queries,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new InventoryItemsTableQuery(
            inventoryId,
            new PageRequest(page, pageSize),
            sortBy,
            sortDescending);
        var result = await queries.GetTableAsync(query, cancellationToken);

        return Ok(result);
    }
}
