using Inventra.Api.Controllers.Requests;
using Inventra.Application.Items.DeleteInventoryItem;
using Inventra.Application.Items.LikeInventoryItem;
using Inventra.Application.Items.UnlikeInventoryItem;
using Inventra.Application.Items.UpdateInventoryItem;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("items/{itemId:guid}")]
public class ItemManagementController : ApiControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UpdateItem(
        Guid itemId,
        [FromBody] UpdateInventoryItemBody body,
        [FromServices] UpdateInventoryItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = body.ToRequest(itemId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteItem(
        Guid itemId,
        [FromQuery] long expectedVersion,
        [FromServices] DeleteInventoryItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new DeleteInventoryItemRequest(itemId, expectedVersion);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPost("like")]
    public async Task<IActionResult> LikeItem(
        Guid itemId,
        [FromServices] LikeInventoryItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new LikeInventoryItemRequest(itemId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpDelete("like")]
    public async Task<IActionResult> UnlikeItem(
        Guid itemId,
        [FromServices] UnlikeInventoryItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new UnlikeInventoryItemRequest(itemId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }
}
