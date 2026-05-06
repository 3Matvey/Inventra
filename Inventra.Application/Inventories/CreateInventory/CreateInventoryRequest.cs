namespace Inventra.Application.Inventories.CreateInventory;

public sealed record CreateInventoryRequest(
    Guid CategoryId,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl,
    IReadOnlyCollection<string> Tags);
