namespace Inventra.Application.Inventories.ReorderInventoryFields;

public sealed record ReorderInventoryFieldsRequest(
    Guid InventoryId,
    IReadOnlyList<Guid> OrderedFieldIds);
