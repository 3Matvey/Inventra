using Inventra.Application.Inventories.UpdateInventoryTags;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventoryTagsBody(
    long ExpectedVersion,
    IReadOnlyCollection<string> Tags)
{
    public UpdateInventoryTagsRequest ToRequest(Guid inventoryId)
    {
        return new UpdateInventoryTagsRequest(
            inventoryId,
            ExpectedVersion,
            Tags);
    }
}
