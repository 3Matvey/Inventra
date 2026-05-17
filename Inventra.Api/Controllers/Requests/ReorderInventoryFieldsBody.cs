namespace Inventra.Api.Controllers.Requests;

public sealed record ReorderInventoryFieldsBody(IReadOnlyList<Guid> OrderedFieldIds);
