namespace Inventra.Domain.Enums;

public enum InventoryIdElementType
{
    FixedText = 1,
    Random20BitNumber = 2,
    Random32BitNumber = 3,
    Random6DigitNumber = 4,
    Random9DigitNumber = 5,
    Guid = 6,
    DateTime = 7,
    Sequence = 8
}
