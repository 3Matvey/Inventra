using Inventra.Application.Common.Queries.Dto;

namespace Inventra.Application.Users.Queries;

public interface IUserQueries
{
    /// <summary>
    /// Finds users by username or email prefix for autocomplete controls.
    /// </summary>
    Task<IReadOnlyCollection<AutocompleteOptionDto>> AutocompleteAsync(
        string term,
        int limit,
        CancellationToken cancellationToken = default);
}
