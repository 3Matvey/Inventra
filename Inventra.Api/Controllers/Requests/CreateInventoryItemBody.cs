using Inventra.Application.Items;

namespace Inventra.Api.Controllers.Requests;

public sealed record CreateInventoryItemBody(
    IReadOnlyCollection<ItemFieldValueRequest> FieldValues);
