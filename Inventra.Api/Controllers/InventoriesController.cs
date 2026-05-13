using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Inventories.Queries;
using Inventra.Application.Inventories.Queries.Dto;
using Inventra.Application.Items.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("inventories")]
public class InventoriesController : ApiControllerBase
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(
        [FromServices] IInventoryQueries queries,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.GetLatestAsync(new PageRequest(page, pageSize), cancellationToken);

        return Ok(result);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop(
        [FromServices] IInventoryQueries queries,
        [FromQuery] int count = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.GetTopByItemsAsync(count, cancellationToken);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromServices] IInventoryQueries queries,
        [FromQuery] string term = "",
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? tagId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new InventorySearchQuery(term, new PageRequest(page, pageSize), categoryId, tagId);
        var result = await queries.SearchAsync(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{inventoryId:guid}")]
    public async Task<IActionResult> GetDetails(
        Guid inventoryId,
        [FromServices] IInventoryQueries queries,
        CancellationToken cancellationToken)
    {
        var result = await queries.GetDetailsAsync(inventoryId, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("owned/{ownerId:guid}")]
    public async Task<IActionResult> GetOwnedByUser(
        Guid ownerId,
        [FromServices] IInventoryQueries queries,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.GetOwnedByUserAsync(ownerId, new PageRequest(page, pageSize), cancellationToken);

        return Ok(result);
    }

    [HttpGet("writable/{userId:guid}")]
    public async Task<IActionResult> GetWritableByUser(
        Guid userId,
        [FromServices] IInventoryQueries queries,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.GetWritableByUserAsync(userId, new PageRequest(page, pageSize), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{inventoryId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(
        Guid inventoryId,
        [FromServices] IInventoryItemQueries queries,
        CancellationToken cancellationToken)
    {
        var result = await queries.GetStatisticsAsync(inventoryId, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
