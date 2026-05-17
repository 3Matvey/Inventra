namespace Inventra.Api.Controllers.Requests;

public sealed record ReorderInventoryFieldsBody(
    long ExpectedVersion,
    IReadOnlyList<Guid> OrderedFieldIds);
