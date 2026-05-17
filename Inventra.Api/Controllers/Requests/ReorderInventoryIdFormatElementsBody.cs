namespace Inventra.Api.Controllers.Requests;

public sealed record ReorderInventoryIdFormatElementsBody(
    long ExpectedVersion,
    IReadOnlyList<Guid> OrderedElementIds);
