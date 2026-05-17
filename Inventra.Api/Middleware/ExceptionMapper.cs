using Inventra.Application.Common.Results;
using Inventra.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Inventra.Api.Middleware;

internal static class ExceptionMapper
{
    public static Error Map(Exception exception, IHostEnvironment environment)
    {
        return exception switch
        {
            DbUpdateConcurrencyException => ConcurrencyConflict(),
            DbUpdateException updateException when IsUniqueViolation(updateException) => UniqueViolation(),
            DbUpdateException updateException when IsForeignKeyViolation(updateException) => ForeignKeyViolation(),
            DomainException domainException => DomainFailure(domainException),
            KeyNotFoundException notFoundException => NotFound(notFoundException),
            ArgumentException argumentException => BadRequest(argumentException),
            _ => Unhandled(exception, environment)
        };
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return GetPostgresException(exception)?.SqlState == PostgresErrorCodes.UniqueViolation;
    }

    private static bool IsForeignKeyViolation(DbUpdateException exception)
    {
        return GetPostgresException(exception)?.SqlState == PostgresErrorCodes.ForeignKeyViolation;
    }

    private static PostgresException? GetPostgresException(DbUpdateException exception)
    {
        return exception.InnerException as PostgresException;
    }

    private static Error ConcurrencyConflict()
    {
        return Error.Conflict(
            "Persistence.ConcurrencyConflict",
            "The resource was modified by another request. Reload it and try again.");
    }

    private static Error UniqueViolation()
    {
        return Error.Conflict(
            "Persistence.UniqueViolation",
            "The database rejected the change because it would create a duplicate value.");
    }

    private static Error ForeignKeyViolation()
    {
        return Error.Conflict(
            "Persistence.ForeignKeyViolation",
            "The database rejected the change because related data is missing or still in use.");
    }

    private static Error DomainFailure(DomainException exception)
    {
        return Error.BadRequest("Domain.ValidationFailed", exception.Message);
    }

    private static Error NotFound(KeyNotFoundException exception)
    {
        return Error.NotFound("Common.NotFound", exception.Message);
    }

    private static Error BadRequest(ArgumentException exception)
    {
        return Error.BadRequest("Common.BadRequest", exception.Message);
    }

    private static Error Unhandled(Exception exception, IHostEnvironment environment)
    {
        return Error.Failure(
            "Common.UnhandledException",
            environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.");
    }
}
