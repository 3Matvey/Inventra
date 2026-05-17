using Inventra.Application.Items;
using Inventra.Application.Items.UpdateInventoryItem;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventoryItemBody(
    long ExpectedVersion,
    string CustomId,
    IReadOnlyCollection<ItemFieldValueRequest> FieldValues)
{
    public UpdateInventoryItemRequest ToRequest(Guid itemId)
    {
        return new UpdateInventoryItemRequest(
            itemId,
            ExpectedVersion,
            CustomId,
            FieldValues);
    }
}
