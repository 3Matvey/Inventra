using Inventra.Application.Inventories.UpdateInventoryField;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventoryFieldBody(
    string Title,
    string? Description,
    bool ShowInTable)
{
    public UpdateInventoryFieldRequest ToRequest(Guid inventoryId, Guid fieldId)
    {
        return new UpdateInventoryFieldRequest(
            inventoryId,
            fieldId,
            Title,
            Description,
            ShowInTable);
    }
}
