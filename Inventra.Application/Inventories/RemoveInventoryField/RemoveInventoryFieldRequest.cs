namespace Inventra.Application.Inventories.RemoveInventoryField;

public sealed record RemoveInventoryFieldRequest(
    Guid InventoryId,
    long ExpectedVersion,
    Guid FieldId);
