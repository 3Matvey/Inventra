namespace Inventra.Application.Inventories.UpdateInventoryField;

public sealed record UpdateInventoryFieldRequest(
    Guid InventoryId,
    long ExpectedVersion,
    Guid FieldId,
    string Title,
    string? Description,
    bool ShowInTable);
