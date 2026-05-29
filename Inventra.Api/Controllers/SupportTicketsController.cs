using Inventra.Application.Support.CreateSupportTicket;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Route("support/tickets")]
public sealed class SupportTicketsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateSupportTicketUseCase useCase,
        [FromBody] CreateSupportTicketRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return FromResult(result);
    }
}
