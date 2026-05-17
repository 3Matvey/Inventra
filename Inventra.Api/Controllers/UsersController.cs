using Inventra.Application.Users.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("users")]
public class UsersController : ApiControllerBase
{
    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete(
        [FromServices] IUserQueries queries,
        [FromQuery] string term,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.AutocompleteAsync(term ?? string.Empty, limit, cancellationToken);

        return Ok(result);
    }
}
