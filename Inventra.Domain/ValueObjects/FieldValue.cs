using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.ValueObjects;

public sealed record FieldValue
{
    public string? TextValue { get; }
    public decimal? NumberValue { get; }
    public bool? BooleanValue { get; }

    private FieldValue(string? textValue, decimal? numberValue, bool? booleanValue)
    {
        TextValue = textValue;
        NumberValue = numberValue;
        BooleanValue = booleanValue;
    }

    public static FieldValue ForText(string? value) => new(value, null, null);

    public static FieldValue ForNumber(decimal? value) => new(null, value, null);

    public static FieldValue ForBoolean(bool value) => new(null, null, value);

    public static FieldValue Empty() => new(null, null, null);

    public void EnsureMatches(InventoryFieldType fieldType)
    {
        var isValid = fieldType switch
        {
            InventoryFieldType.SingleLineText => NumberValue is null && BooleanValue is null,
            InventoryFieldType.MultiLineText => NumberValue is null && BooleanValue is null,
            InventoryFieldType.Link => NumberValue is null && BooleanValue is null,
            InventoryFieldType.Number => TextValue is null && BooleanValue is null,
            InventoryFieldType.Boolean => TextValue is null && NumberValue is null && BooleanValue.HasValue,
            _ => false
        };

        if (!isValid)
        {
            throw new InvalidItemFieldValueException($"Field value does not match field type '{fieldType}'.");
        }
    }
}
