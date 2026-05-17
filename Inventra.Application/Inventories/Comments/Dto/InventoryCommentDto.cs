namespace Inventra.Application.Inventories.Comments.Dto;

/// <summary>
/// Represents a discussion comment displayed on an inventory page.
/// </summary>
public sealed record InventoryCommentDto(
    Guid Id,
    Guid InventoryId,
    Guid AuthorId,
    string AuthorUserName,
    string BodyMarkdown,
    DateTimeOffset CreatedAt);
