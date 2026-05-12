namespace Inventra.Application.Common.Interfaces;

public interface ICustomIdGenerator
{
    string Generate(
        Inventory inventory,
        long? sequenceNumber,
        DateTimeOffset createdAt);
}
