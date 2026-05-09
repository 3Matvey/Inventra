namespace Inventra.Application.Common.Interfaces;

public interface IInventorySequenceProvider
{
    Task<long> GetNextSequenceAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default);
}
