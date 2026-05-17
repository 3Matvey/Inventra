using Inventra.Application.Common.Results;

namespace Inventra.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var error = ExceptionMapper.Map(exception, environment);
            Log(exception, error);
            await WriteAsync(context, error);
        }
    }

    private void Log(Exception exception, Error error)
    {
        if (error.ErrorType == ErrorType.Failure)
            logger.LogError(exception, "Unhandled exception occurred.");
        else
            logger.LogWarning(exception, "Request failed with {Code}.", error.Code);
    }

    private static async Task WriteAsync(HttpContext context, Error error)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ToStatusCode(error);

        await context.Response.WriteAsJsonAsync(error);
    }

    private static int ToStatusCode(Error error)
    {
        return error.ErrorType switch
        {
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.AccessUnauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.AccessForbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
