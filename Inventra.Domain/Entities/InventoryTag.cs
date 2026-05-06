using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class InventoryTag : Entity
{
    public Guid InventoryId { get; private set; }
    public Guid TagId { get; private set; }

    private InventoryTag()
    {
    }

    public InventoryTag(Guid inventoryId, Guid tagId)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        TagId = Guard.RequiredId(tagId);
    }
}
