using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Application.Categories.Queries;


public interface ICategoryQueries
{
    /// <summary>
    /// Gets all inventory categories ordered by name.
    /// </summary>
    Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
