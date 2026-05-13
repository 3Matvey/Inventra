using Inventra.Application.Items.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("items")]
public class ItemsController : ApiControllerBase
{
    [HttpGet("{itemId:guid}")]
    public async Task<IActionResult> GetDetails(
        Guid itemId,
        [FromServices] IInventoryItemQueries queries,
        CancellationToken cancellationToken)
    {
        var result = await queries.GetDetailsAsync(itemId, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
