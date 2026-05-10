namespace Inventra.Application.Items.CreateInventoryItem;

public sealed record CreateInventoryItemRequest(
    Guid InventoryId,
    IReadOnlyCollection<ItemFieldValueRequest> FieldValues);
