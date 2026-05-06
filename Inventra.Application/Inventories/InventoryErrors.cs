using Inventra.Application.Common.Results;

namespace Inventra.Application.Inventories;

public static class InventoryErrors
{
    public static Error AuthenticationRequired() =>
        Error.AccessUnauthorized(
            "Inventory.AuthenticationRequired",
            "Authentication is required to manage inventories.");

    public static Error CategoryNotFound(Guid categoryId) =>
        Error.BadRequest(
            "Inventory.CategoryNotFound",
            $"Category '{categoryId}' was not found.");

    public static Error NotFound(Guid inventoryId) =>
        Error.NotFound(
            "Inventory.NotFound",
            $"Inventory '{inventoryId}' was not found.");

    public static Error AccessDenied() =>
        Error.AccessForbidden(
            "Inventory.AccessDenied",
            "You do not have permission to manage this inventory.");

    public static Error FieldNotFound(Guid fieldId) =>
        Error.NotFound(
            "Inventory.FieldNotFound",
            $"Inventory field '{fieldId}' was not found.");

    public static Error InvalidFieldOrder() =>
        Error.BadRequest(
            "Inventory.InvalidFieldOrder",
            "Field order must contain every field exactly once.");

    public static Error FieldLimitExceeded(string description) =>
        Error.BadRequest(
            "Inventory.FieldLimitExceeded",
            description);
}
