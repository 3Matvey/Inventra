using Inventra.Application.Inventories.UpdateInventorySettings;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventorySettingsBody(
    long ExpectedVersion,
    Guid CategoryId,
    string Title,
    string? DescriptionMarkdown,
    string? ImageUrl,
    string? ImagePublicId)
{
    public UpdateInventorySettingsRequest ToRequest(Guid inventoryId)
    {
        return new UpdateInventorySettingsRequest(
            inventoryId,
            ExpectedVersion,
            CategoryId,
            Title,
            DescriptionMarkdown,
            ImageUrl,
            ImagePublicId);
    }
}
