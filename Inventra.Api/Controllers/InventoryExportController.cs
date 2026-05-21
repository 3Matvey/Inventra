using Inventra.Application.Common.Results;
using Inventra.Application.Inventories.Exports;
using Inventra.Application.Inventories.Exports.Dto;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Inventra.Api.Controllers;

[Route("inventories/{inventoryId:guid}/export")]
public class InventoryExportController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Export(
        Guid inventoryId,
        [FromServices] ExportInventoryUseCase useCase,
        [FromServices] IInventoryExportFileWriter writer,
        [FromQuery] string format = "csv",
        CancellationToken cancellationToken = default)
    {
        var request = new ExportInventoryRequest(inventoryId, format);
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return FromError(result.Error);

        await WriteFileAsync(result.Value, writer, cancellationToken);

        return new EmptyResult();
    }

    private async Task WriteFileAsync(
        InventoryExportFileDto file,
        IInventoryExportFileWriter writer,
        CancellationToken cancellationToken)
    {
        Response.ContentType = file.ContentType;
        Response.GetTypedHeaders().ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileNameStar = file.FileName
        };

        await writer.WriteAsync(file.Export, file.Format, Response.Body, cancellationToken);
    }
}
