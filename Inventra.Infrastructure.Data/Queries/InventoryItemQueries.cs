using Inventra.Application.Items.Queries;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryItemQueries(
    AppDbContext dbContext,
    ICurrentUser currentUser) : IInventoryItemQueries
{
}
