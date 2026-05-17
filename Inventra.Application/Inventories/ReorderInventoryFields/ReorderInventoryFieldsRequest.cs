namespace Inventra.Application.Inventories.ReorderInventoryFields;

public sealed record ReorderInventoryFieldsRequest(
    Guid InventoryId,
    long ExpectedVersion,
    IReadOnlyList<Guid> OrderedFieldIds);
