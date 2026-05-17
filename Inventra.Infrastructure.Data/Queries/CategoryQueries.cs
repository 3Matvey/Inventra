using Inventra.Application.Categories.Queries;
using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal class CategoryQueries(AppDbContext dbContext) : ICategoryQueries
{
    public async Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = dbContext.Categories.AsNoTracking();

        return await categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name))
            .ToArrayAsync(cancellationToken);
    }
}
