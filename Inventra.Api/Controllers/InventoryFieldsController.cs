using Inventra.Api.Controllers.Requests;
using Inventra.Application.Inventories.AddInventoryField;
using Inventra.Application.Inventories.RemoveInventoryField;
using Inventra.Application.Inventories.ReorderInventoryFields;
using Inventra.Application.Inventories.UpdateInventoryField;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("inventories/{inventoryId:guid}/fields")]
public class InventoryFieldsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddField(
        Guid inventoryId,
        [FromBody] AddInventoryFieldBody body,
        [FromServices] AddInventoryFieldUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = body.ToRequest(inventoryId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPut("{fieldId:guid}")]
    public async Task<IActionResult> UpdateField(
        Guid inventoryId,
        Guid fieldId,
        [FromBody] UpdateInventoryFieldBody body,
        [FromServices] UpdateInventoryFieldUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = body.ToRequest(inventoryId, fieldId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpDelete("{fieldId:guid}")]
    public async Task<IActionResult> RemoveField(
        Guid inventoryId,
        Guid fieldId,
        [FromServices] RemoveInventoryFieldUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new RemoveInventoryFieldRequest(inventoryId, fieldId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPut("order")]
    public async Task<IActionResult> ReorderFields(
        Guid inventoryId,
        [FromBody] ReorderInventoryFieldsBody body,
        [FromServices] ReorderInventoryFieldsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new ReorderInventoryFieldsRequest(inventoryId, body.OrderedFieldIds);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }
}
