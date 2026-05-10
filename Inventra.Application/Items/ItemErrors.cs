using Inventra.Application.Common.Results;

namespace Inventra.Application.Items;

public static class ItemErrors
{
    public static Error AuthenticationRequired() =>
        Error.AccessUnauthorized(
            "Item.AuthenticationRequired",
            "Authentication is required to modify inventory items.");

    public static Error NotFound(Guid itemId) =>
        Error.NotFound(
            "Item.NotFound",
            $"Inventory item '{itemId}' was not found.");

    public static Error InventoryNotFound(Guid inventoryId) =>
        Error.NotFound(
            "Item.InventoryNotFound",
            $"Inventory '{inventoryId}' was not found.");

    public static Error AccessDenied() =>
        Error.AccessForbidden(
            "Item.AccessDenied",
            "You do not have permission to modify items in this inventory.");

    public static Error FieldNotFound(Guid fieldId) =>
        Error.BadRequest(
            "Item.FieldNotFound",
            $"Inventory field '{fieldId}' was not found.");

    public static Error InvalidFieldValue(string description) =>
        Error.BadRequest(
            "Item.InvalidFieldValue",
            description);

    public static Error CustomIdFormatNotConfigured() =>
        Error.BadRequest(
            "Item.CustomIdFormatNotConfigured",
            "Inventory custom ID format is not configured.");
}
