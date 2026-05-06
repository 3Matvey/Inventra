using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class ItemLike : Entity
{
    public Guid ItemId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ItemLike()
    {
    }

    public ItemLike(Guid itemId, Guid userId, DateTimeOffset createdAt)
    {
        ItemId = Guard.RequiredId(itemId);
        UserId = Guard.RequiredId(userId);
        CreatedAt = createdAt;
    }
}
