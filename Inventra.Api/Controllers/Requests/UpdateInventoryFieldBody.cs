using Inventra.Application.Inventories.UpdateInventoryField;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventoryFieldBody(
    long ExpectedVersion,
    string Title,
    string? Description,
    bool ShowInTable)
{
    public UpdateInventoryFieldRequest ToRequest(Guid inventoryId, Guid fieldId)
    {
        return new UpdateInventoryFieldRequest(
            inventoryId,
            ExpectedVersion,
            fieldId,
            Title,
            Description,
            ShowInTable);
    }
}
