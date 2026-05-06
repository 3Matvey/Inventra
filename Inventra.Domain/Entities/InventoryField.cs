using Inventra.Domain.Common;
using Inventra.Domain.Enums;

namespace Inventra.Domain.Entities;

public sealed class InventoryField : Entity
{
    public Guid InventoryId { get; private set; }
    public InventoryFieldType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool ShowInTable { get; private set; }
    public int Order { get; private set; }

    private InventoryField()
    {
    }

    public InventoryField(
        Guid inventoryId,
        InventoryFieldType type,
        string title,
        string? description,
        bool showInTable,
        int order)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        Type = type;
        Update(title, description, showInTable);
        MoveTo(order);
    }

    public void Update(string title, string? description, bool showInTable)
    {
        Title = Guard.Required(title);
        Description = Guard.Optional(description);
        ShowInTable = showInTable;
    }

    public void MoveTo(int order)
    {
        Order = Guard.NonNegative(order);
    }
}
