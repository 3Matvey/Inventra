using Inventra.Domain.Entities;

namespace Inventra.Application.Common.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Tag>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Tag>> SearchByPrefixAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default);

    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
}
