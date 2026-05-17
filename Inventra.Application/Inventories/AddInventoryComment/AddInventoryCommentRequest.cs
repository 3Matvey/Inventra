namespace Inventra.Application.Inventories.AddInventoryComment;

public sealed record AddInventoryCommentRequest(
    Guid InventoryId,
    string BodyMarkdown);
