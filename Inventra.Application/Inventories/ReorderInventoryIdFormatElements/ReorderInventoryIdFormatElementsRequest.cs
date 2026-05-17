namespace Inventra.Application.Inventories.ReorderInventoryIdFormatElements;

public sealed record ReorderInventoryIdFormatElementsRequest(
    Guid InventoryId,
    long ExpectedVersion,
    IReadOnlyList<Guid> OrderedElementIds);
