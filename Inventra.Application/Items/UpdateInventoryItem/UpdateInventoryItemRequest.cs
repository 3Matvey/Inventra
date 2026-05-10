namespace Inventra.Application.Items.UpdateInventoryItem;

public sealed record UpdateInventoryItemRequest(
    Guid ItemId,
    string CustomId,
    IReadOnlyCollection<ItemFieldValueRequest> FieldValues);
