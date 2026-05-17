using Inventra.Api.Controllers.Requests;
using Inventra.Application.Inventories.AddInventoryIdFormatElement;
using Inventra.Application.Inventories.PreviewInventoryCustomId;
using Inventra.Application.Inventories.RemoveInventoryIdFormatElement;
using Inventra.Application.Inventories.ReorderInventoryIdFormatElements;
using Inventra.Application.Inventories.UpdateInventoryIdFormatElement;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("inventories/{inventoryId:guid}/id-format")]
public class InventoryIdFormatController : ApiControllerBase
{
    [HttpPost("elements")]
    public async Task<IActionResult> AddElement(
        Guid inventoryId,
        [FromBody] AddInventoryIdFormatElementBody body,
        [FromServices] AddInventoryIdFormatElementUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = body.ToRequest(inventoryId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPut("elements/{elementId:guid}")]
    public async Task<IActionResult> UpdateElement(
        Guid inventoryId,
        Guid elementId,
        [FromBody] UpdateInventoryIdFormatElementBody body,
        [FromServices] UpdateInventoryIdFormatElementUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = body.ToRequest(inventoryId, elementId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpDelete("elements/{elementId:guid}")]
    public async Task<IActionResult> RemoveElement(
        Guid inventoryId,
        Guid elementId,
        [FromQuery] long expectedVersion,
        [FromServices] RemoveInventoryIdFormatElementUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new RemoveInventoryIdFormatElementRequest(
            inventoryId,
            expectedVersion,
            elementId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPut("elements/order")]
    public async Task<IActionResult> ReorderElements(
        Guid inventoryId,
        [FromBody] ReorderInventoryIdFormatElementsBody body,
        [FromServices] ReorderInventoryIdFormatElementsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new ReorderInventoryIdFormatElementsRequest(
            inventoryId,
            body.ExpectedVersion,
            body.OrderedElementIds);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpGet("preview")]
    public async Task<IActionResult> Preview(
        Guid inventoryId,
        [FromServices] PreviewInventoryCustomIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new PreviewInventoryCustomIdRequest(inventoryId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }
}
