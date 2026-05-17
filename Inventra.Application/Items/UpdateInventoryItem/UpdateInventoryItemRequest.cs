namespace Inventra.Application.Items.UpdateInventoryItem;

public sealed record UpdateInventoryItemRequest(
    Guid ItemId,
    long ExpectedVersion,
    string CustomId,
    IReadOnlyCollection<ItemFieldValueRequest> FieldValues);
