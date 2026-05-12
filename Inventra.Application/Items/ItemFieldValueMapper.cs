using Inventra.Domain.Enums;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.Items;

internal static class ItemFieldValueMapper
{
    public static Result<FieldValue> Map(
        InventoryField field,
        ItemFieldValueRequest request)
    {
        return field.Type switch
        {
            InventoryFieldType.SingleLineText => FieldValue.ForText(request.TextValue),
            InventoryFieldType.MultiLineText => FieldValue.ForText(request.TextValue),
            InventoryFieldType.Link => FieldValue.ForText(request.TextValue),
            InventoryFieldType.Number => FieldValue.ForNumber(request.NumberValue),
            InventoryFieldType.Boolean => MapBoolean(request),
            _ => ItemErrors.InvalidFieldValue($"Unsupported field type '{field.Type}'.")
        };
    }

    private static Result<FieldValue> MapBoolean(ItemFieldValueRequest request)
    {
        return request.BooleanValue.HasValue
            ? FieldValue.ForBoolean(request.BooleanValue.Value)
            : ItemErrors.InvalidFieldValue("Boolean field value is required.");
    }
}
