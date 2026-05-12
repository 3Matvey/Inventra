using Inventra.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult(Result result)
    {
        return result.IsSuccess
            ? NoContent()
            : FromError(result.Error);
    }

    protected IActionResult FromResult<TValue>(Result<TValue> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : FromError(result.Error);
    }

    private IActionResult FromError(Error? error)
    {
        if (error is null)
            return Problem();

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
