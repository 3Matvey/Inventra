using Inventra.Api.Controllers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("health")]
public class HealthController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new HealthResponse("ok", DateTimeOffset.UtcNow));
    }
}
