namespace Inventra.Application.Inventories.UpdateInventorySettings;

public sealed record UpdateInventorySettingsRequest(
    Guid InventoryId,
    Guid CategoryId,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl);
