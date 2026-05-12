using Inventra.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult(Result result)
    {
        return result.IsSuccess
            ? NoContent()
            : ToErrorResult(result.Error);
    }

    protected IActionResult ToActionResult<TValue>(Result<TValue> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : ToErrorResult(result.Error);
    }

    private IActionResult ToErrorResult(Error? error)
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
