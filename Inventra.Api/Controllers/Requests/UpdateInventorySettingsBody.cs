using Inventra.Application.Inventories.UpdateInventorySettings;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventorySettingsBody(
    Guid CategoryId,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl)
{
    public UpdateInventorySettingsRequest ToRequest(Guid inventoryId)
    {
        return new UpdateInventorySettingsRequest(
            inventoryId,
            CategoryId,
            Title,
            DescriptionMarkdown,
            ImageUrl);
    }
}
