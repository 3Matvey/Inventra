namespace Inventra.Application.Inventories.UpdateInventorySettings;

public sealed record UpdateInventorySettingsRequest(
    Guid InventoryId,
    long ExpectedVersion,
    Guid CategoryId,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl);
