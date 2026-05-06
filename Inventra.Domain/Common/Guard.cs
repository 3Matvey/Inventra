using System.Runtime.CompilerServices;

namespace Inventra.Domain.Common;

public static class Guard
{
    public static T Required<T>(
        T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : class
    {
        return value ?? throw new ArgumentNullException(parameterName);
    }

    public static Guid RequiredId(
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        return value == Guid.Empty
            ? throw new ArgumentException($"{parameterName} is required.", parameterName)
            : value;
    }

    public static string Required(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{parameterName} is required.", parameterName)
            : value.Trim();
    }

    public static string? Optional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static int NonNegative(
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        return value < 0
            ? throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be negative.")
            : value;
    }

    public static IReadOnlyCollection<Guid> RequiredIds(
        IEnumerable<Guid>? values,
        [CallerArgumentExpression(nameof(values))] string? parameterName = null)
    {
        var ids = Required(values).ToArray();

        if (ids.Any(x => x == Guid.Empty))
            throw new ArgumentException($"{parameterName} cannot contain empty ids.", parameterName);

        return ids.Distinct().ToArray();
    }

    public static IReadOnlyList<Guid> RequiredCompleteIdSet(
        IReadOnlyList<Guid>? values,
        int expectedCount,
        [CallerArgumentExpression(nameof(values))] string? parameterName = null)
    {
        var ids = Required(values);

        if (ids.Any(x => x == Guid.Empty))
            throw new ArgumentException($"{parameterName} cannot contain empty ids.", parameterName);

        if (ids.Count != expectedCount || ids.Distinct().Count() != expectedCount)
            throw new ArgumentException($"{parameterName} must contain every id exactly once.", parameterName);

        return ids;
    }
}
