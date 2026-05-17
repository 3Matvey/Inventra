namespace Inventra.Api.Controllers.Requests;

public sealed record ReorderInventoryIdFormatElementsBody(IReadOnlyList<Guid> OrderedElementIds);
