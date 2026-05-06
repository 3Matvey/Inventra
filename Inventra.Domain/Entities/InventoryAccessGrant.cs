using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class InventoryAccessGrant : Entity
{
    public Guid InventoryId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset GrantedAt { get; private set; }

    private InventoryAccessGrant()
    {
    }

    public InventoryAccessGrant(Guid inventoryId, Guid userId, DateTimeOffset grantedAt)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        UserId = Guard.RequiredId(userId);
        GrantedAt = grantedAt;
    }
}
