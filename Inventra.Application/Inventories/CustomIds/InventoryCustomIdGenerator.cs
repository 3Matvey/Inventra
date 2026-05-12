namespace Inventra.Application.Inventories.CustomIds;

public sealed class InventoryCustomIdGenerator : ICustomIdGenerator
{
    public string Generate(
        Inventory inventory,
        long? sequenceNumber,
        DateTimeOffset createdAt)
    {
        var context = new CustomIdComposeContext(createdAt, sequenceNumber, UseSampleValues: false);

        return InventoryCustomIdComposer.Compose(inventory.IdFormatElements, context);
    }
}
