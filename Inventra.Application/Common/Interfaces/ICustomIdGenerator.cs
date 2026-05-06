using Inventra.Domain.Entities;

namespace Inventra.Application.Common.Interfaces;

public interface ICustomIdGenerator
{
    Task<string> GenerateAsync(Inventory inventory, CancellationToken cancellationToken = default);
}
