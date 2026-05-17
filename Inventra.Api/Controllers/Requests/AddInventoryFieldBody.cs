using Inventra.Application.Inventories.AddInventoryField;
using Inventra.Domain.Enums;

namespace Inventra.Api.Controllers.Requests;

public sealed record AddInventoryFieldBody(
    long ExpectedVersion,
    InventoryFieldType Type,
    string Title,
    string? Description,
    bool ShowInTable)
{
    public AddInventoryFieldRequest ToRequest(Guid inventoryId)
    {
        return new AddInventoryFieldRequest(
            inventoryId,
            ExpectedVersion,
            Type,
            Title,
            Description,
            ShowInTable);
    }
}
