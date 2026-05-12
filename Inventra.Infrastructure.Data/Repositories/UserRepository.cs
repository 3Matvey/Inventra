namespace Inventra.Infrastructure.Data.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<UserAccount?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.UserAccounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserAccount>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UserAccounts
            .OrderBy(x => x.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
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

    public async Task AddAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        await dbContext.UserAccounts.AddAsync(user, cancellationToken);
    }

    public void Remove(UserAccount user)
    {
        dbContext.UserAccounts.Remove(user);
    }
}
