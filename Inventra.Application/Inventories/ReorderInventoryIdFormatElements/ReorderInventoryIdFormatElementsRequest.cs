namespace Inventra.Application.Inventories.ReorderInventoryIdFormatElements;

public sealed record ReorderInventoryIdFormatElementsRequest(
    Guid InventoryId,
    IReadOnlyList<Guid> OrderedElementIds);
