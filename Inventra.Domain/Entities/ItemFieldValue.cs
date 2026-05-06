using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class ItemFieldValue : Entity
{
    public Guid ItemId { get; private set; }
    public Guid FieldId { get; private set; }
    public string? TextValue { get; private set; }
    public decimal? NumberValue { get; private set; }
    public bool? BooleanValue { get; private set; }

    private ItemFieldValue()
    {
    }

    public ItemFieldValue(Guid itemId, Guid fieldId, InventoryFieldType fieldType, FieldValue value)
    {
        ItemId = Guard.RequiredId(itemId);
        FieldId = Guard.RequiredId(fieldId);
        SetValue(fieldType, value);
    }

    public void SetValue(InventoryFieldType fieldType, FieldValue value)
    {
        value.EnsureMatches(fieldType);

        TextValue = value.TextValue;
        NumberValue = value.NumberValue;
        BooleanValue = value.BooleanValue;
    }
}
