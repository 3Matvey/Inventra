using Inventra.Application.Inventories.AddInventoryField;
using Inventra.Domain.Enums;

namespace Inventra.Api.Controllers.Requests;

public sealed record AddInventoryFieldBody(
    InventoryFieldType Type,
    string Title,
    string? Description,
    bool ShowInTable)
{
    public AddInventoryFieldRequest ToRequest(Guid inventoryId)
    {
        return new AddInventoryFieldRequest(
            inventoryId,
            Type,
            Title,
            Description,
            ShowInTable);
    }
}
