using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class InventoryComment : Entity
{
    public Guid InventoryId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string BodyMarkdown { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private InventoryComment()
    {
    }

    public InventoryComment(Guid inventoryId, Guid authorId, string bodyMarkdown, DateTimeOffset createdAt)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        AuthorId = Guard.RequiredId(authorId);
        CreatedAt = createdAt;
        UpdateBody(bodyMarkdown);
    }

    public void UpdateBody(string bodyMarkdown)
    {
        BodyMarkdown = Guard.Required(bodyMarkdown);
    }
}
