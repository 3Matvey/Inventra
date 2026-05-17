using Inventra.Api.Controllers.Requests;
using Inventra.Application.Items.CreateInventoryItem;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("inventories/{inventoryId:guid}/items")]
public class InventoryItemManagementController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateItem(
        Guid inventoryId,
        [FromBody] CreateInventoryItemBody body,
        [FromServices] CreateInventoryItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new CreateInventoryItemRequest(inventoryId, body.FieldValues);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }
}
