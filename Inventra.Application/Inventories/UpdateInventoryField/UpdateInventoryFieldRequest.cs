namespace Inventra.Application.Inventories.UpdateInventoryField;

public sealed record UpdateInventoryFieldRequest(
    Guid InventoryId,
    Guid FieldId,
    string Title,
    string? Description,
    bool ShowInTable);
