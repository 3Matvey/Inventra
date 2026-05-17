using Inventra.Api.Controllers.Requests;
using Inventra.Api.Hubs;
using Inventra.Application.Common.Queries.Dto;
using Inventra.Application.Common.Results;
using Inventra.Application.Inventories.AddInventoryComment;
using Inventra.Application.Inventories.Comments.Dto;
using Inventra.Application.Inventories.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Inventra.Api.Controllers;

[Route("inventories/{inventoryId:guid}/comments")]
public class InventoryCommentsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetComments(
        Guid inventoryId,
        [FromServices] IInventoryCommentQueries queries,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await queries.GetPageAsync(
            inventoryId,
            new PageRequest(page, pageSize),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(
        Guid inventoryId,
        [FromBody] AddInventoryCommentBody body,
        [FromServices] AddInventoryCommentUseCase useCase,
        [FromServices] IHubContext<InventoryDiscussionHub> hubContext,
        CancellationToken cancellationToken)
    {
        var request = new AddInventoryCommentRequest(inventoryId, body.BodyMarkdown);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return await result.Match(
            comment => NotifyCommentAddedAsync(comment, hubContext, cancellationToken),
            error => Task.FromResult(FromError(error)));
    }

    private static async Task<IActionResult> NotifyCommentAddedAsync(
        InventoryCommentDto comment,
        IHubContext<InventoryDiscussionHub> hubContext,
        CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .Group(InventoryDiscussionGroups.ForInventory(comment.InventoryId))
            .SendAsync("commentAdded", comment, cancellationToken);

        return new OkObjectResult(comment);
    }
}
