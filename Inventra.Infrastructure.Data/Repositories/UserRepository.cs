using Inventra.Application.Common.Interfaces;
using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Data.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<UserAccount?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.UserAccounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<UserAccount?> GetByUserNameOrEmailAsync(
        string userNameOrEmail,
        CancellationToken cancellationToken = default)
    {
        return dbContext.UserAccounts.FirstOrDefaultAsync(
            x => x.UserName == userNameOrEmail || x.Email == userNameOrEmail,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserAccount>> SearchByUserNameOrEmailAsync(
        string term,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UserAccounts
            .Where(x => x.UserName.StartsWith(term) || x.Email.StartsWith(term))
            .OrderBy(x => x.UserName)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
    }
}
