using Inventra.Application.Inventories.CreateInventory;
using Inventra.Application.Inventories.GrantInventoryAccess;
using Inventra.Application.Inventories.RevokeInventoryAccess;
using Inventra.Application.Inventories.SetPublicWriteAccess;
using Inventra.Application.Inventories.UpdateInventorySettings;
using Inventra.Api.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("inventories")]
public class InventoryManagementController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInventoryRequest request,
        [FromServices] CreateInventoryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPut("{inventoryId:guid}/settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid inventoryId,
        [FromBody] UpdateInventorySettingsBody body,
        [FromServices] UpdateInventorySettingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = body.ToRequest(inventoryId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPut("{inventoryId:guid}/public-access")]
    public async Task<IActionResult> SetPublicAccess(
        Guid inventoryId,
        [FromBody] SetPublicWriteAccessBody body,
        [FromServices] SetPublicWriteAccessUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new SetPublicWriteAccessRequest(
            inventoryId,
            body.ExpectedVersion,
            body.IsPublic);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpPost("{inventoryId:guid}/access-grants")]
    public async Task<IActionResult> GrantAccess(
        Guid inventoryId,
        [FromBody] GrantInventoryAccessBody body,
        [FromServices] GrantInventoryAccessUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new GrantInventoryAccessRequest(
            inventoryId,
            body.ExpectedVersion,
            body.UserNameOrEmail);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }

    [HttpDelete("{inventoryId:guid}/access-grants/{userId:guid}")]
    public async Task<IActionResult> RevokeAccess(
        Guid inventoryId,
        Guid userId,
        [FromQuery] long expectedVersion,
        [FromServices] RevokeInventoryAccessUseCase useCase,
        CancellationToken cancellationToken)
    {
        var request = new RevokeInventoryAccessRequest(inventoryId, expectedVersion, userId);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }
}
