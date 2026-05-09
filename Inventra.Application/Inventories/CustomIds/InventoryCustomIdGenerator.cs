using Inventra.Application.Common.Interfaces;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.CustomIds;

public sealed class InventoryCustomIdGenerator(
    IDateTimeProvider dateTimeProvider,
    IInventorySequenceProvider sequenceProvider) : ICustomIdGenerator
{
    public async Task<string> GenerateAsync(
        Inventory inventory,
        CancellationToken cancellationToken = default)
    {
        var sequence = await sequenceProvider.GetNextSequenceAsync(inventory.Id, cancellationToken);
        var context = new CustomIdComposeContext(dateTimeProvider.UtcNow, sequence, UseSampleValues: false);

        return InventoryCustomIdComposer.Compose(inventory.IdFormatElements, context);
    }
}
