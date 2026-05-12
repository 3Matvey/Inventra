namespace Inventra.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserAccount>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserAccount?> GetByUserNameOrEmailAsync(
        string userNameOrEmail,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserAccount>> SearchByUserNameOrEmailAsync(
        string term,
        int limit,
        CancellationToken cancellationToken = default);

    Task AddAsync(UserAccount user, CancellationToken cancellationToken = default);

    void Remove(UserAccount user);
}
