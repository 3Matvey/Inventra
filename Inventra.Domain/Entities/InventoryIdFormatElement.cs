using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class InventoryIdFormatElement : Entity
{
    public Guid InventoryId { get; private set; }
    public InventoryIdElementType Type { get; private set; }
    public string? Value { get; private set; }
    public string? Format { get; private set; }
    public int Order { get; private set; }

    private InventoryIdFormatElement()
    {
    }

    public InventoryIdFormatElement(
        Guid inventoryId,
        InventoryIdElementType type,
        string? value,
        string? format,
        int order)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        Type = type;
        Update(value, format);
        MoveTo(order);
    }

    public void Update(string? value, string? format)
    {
        if (Type == InventoryIdElementType.FixedText && string.IsNullOrEmpty(value))
            throw new InvalidInventoryIdFormatException("Fixed text ID element requires a value.");

        Value = value;
        Format = Guard.Optional(format);
    }

    public void MoveTo(int order)
    {
        Order = Guard.NonNegative(order);
    }
}
