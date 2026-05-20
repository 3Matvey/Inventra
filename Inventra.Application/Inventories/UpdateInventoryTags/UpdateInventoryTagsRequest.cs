namespace Inventra.Application.Inventories.UpdateInventoryTags;

public sealed record UpdateInventoryTagsRequest(
    Guid InventoryId,
    long ExpectedVersion,
    IReadOnlyCollection<string> Tags);
