namespace Inventra.Application.Inventories.RemoveInventoryField;

public sealed record RemoveInventoryFieldRequest(
    Guid InventoryId,
    Guid FieldId);
