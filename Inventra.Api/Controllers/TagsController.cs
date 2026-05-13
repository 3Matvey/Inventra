using Inventra.Application.Inventories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("tags")]
public class TagsController : ApiControllerBase
{
    [HttpGet("cloud")]
    public async Task<IActionResult> GetCloud(
        [FromServices] IInventoryQueries queries,
        [FromQuery] int count = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.GetTagCloudAsync(count, cancellationToken);

        return Ok(result);
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete(
        [FromServices] IInventoryQueries queries,
        [FromQuery] string term,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.AutocompleteTagsAsync(term ?? string.Empty, limit, cancellationToken);

        return Ok(result);
    }
}
