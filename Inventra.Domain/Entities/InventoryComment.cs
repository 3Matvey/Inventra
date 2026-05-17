using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class InventoryComment : AuditableEntity
{
    public Guid InventoryId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string BodyMarkdown { get; private set; } = string.Empty;

    private InventoryComment()
    {
    }

    public InventoryComment(Guid inventoryId, Guid authorId, string bodyMarkdown)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        AuthorId = Guard.RequiredId(authorId);
        UpdateBody(bodyMarkdown);
    }

    public void UpdateBody(string bodyMarkdown)
    {
        BodyMarkdown = Guard.Required(bodyMarkdown);
    }
}
