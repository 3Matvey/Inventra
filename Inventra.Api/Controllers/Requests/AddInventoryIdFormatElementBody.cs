using Inventra.Application.Inventories.AddInventoryIdFormatElement;
using Inventra.Domain.Enums;

namespace Inventra.Api.Controllers.Requests;

public sealed record AddInventoryIdFormatElementBody(
    InventoryIdElementType Type,
    string? Value,
    string? Format)
{
    public AddInventoryIdFormatElementRequest ToRequest(Guid inventoryId)
    {
        return new AddInventoryIdFormatElementRequest(
            inventoryId,
            Type,
            Value,
            Format);
    }
}
