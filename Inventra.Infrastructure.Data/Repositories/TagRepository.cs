namespace Inventra.Infrastructure.Data.Repositories;

internal class TagRepository(AppDbContext dbContext) : ITagRepository
{
    public Task<Tag?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Tags.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Tags
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> SearchByPrefixAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Tags
            .Where(x => x.Name.StartsWith(prefix))
            .OrderBy(x => x.Name)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(
        Tag tag,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Tags.AddAsync(tag, cancellationToken);
    }
}
