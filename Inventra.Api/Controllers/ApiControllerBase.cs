using Inventra.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult(Result result)
    {
        return result.Match(
            NoContent,
            FromError);
    }

    protected IActionResult FromResult<TValue>(Result<TValue> result)
    {
        return result.Match(
            value => Ok(value),
            FromError);
    }

    protected IActionResult FromError(Error error)
    {
        return error.ErrorType switch
        {
            ErrorType.BadRequest => BadRequest(error),
            ErrorType.NotFound => NotFound(error),
            ErrorType.Conflict => Conflict(error),
            ErrorType.AccessUnauthorized => Unauthorized(error),
            ErrorType.AccessForbidden => Forbid(),
            _ => Problem(error.Description)
        };
    }
}
