using Inventra.Application.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("categories")]
public class CategoriesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] ICategoryQueries queries,
        CancellationToken cancellationToken)
    {
        var result = await queries.GetAllAsync(cancellationToken);

        return Ok(result);
    }
}
