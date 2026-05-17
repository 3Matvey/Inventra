using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Users.Queries;

namespace Inventra.Infrastructure.Data.Queries;

internal class UserQueries(AppDbContext dbContext) : IUserQueries
{
    public async Task<IReadOnlyCollection<AutocompleteOptionDto>> AutocompleteAsync(
        string term,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var users = dbContext.UserAccounts.AsNoTracking();
        var pattern = term.Trim() + "%";

        return await users
            .Where(x => EF.Functions.ILike(x.UserName, pattern) || EF.Functions.ILike(x.Email, pattern))
            .OrderBy(x => x.UserName)
            .Take(Math.Clamp(limit, 1, 50))
            .Select(x => new AutocompleteOptionDto(x.Id, x.UserName, x.Email))
            .ToArrayAsync(cancellationToken);
    }
}
