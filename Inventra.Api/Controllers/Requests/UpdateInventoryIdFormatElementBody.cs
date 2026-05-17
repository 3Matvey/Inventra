using Inventra.Application.Inventories.UpdateInventoryIdFormatElement;

namespace Inventra.Api.Controllers.Requests;

public sealed record UpdateInventoryIdFormatElementBody(
    string? Value,
    string? Format)
{
    public UpdateInventoryIdFormatElementRequest ToRequest(
        Guid inventoryId,
        Guid elementId)
    {
        return new UpdateInventoryIdFormatElementRequest(
            inventoryId,
            elementId,
            Value,
            Format);
    }
}
