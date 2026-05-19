using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Common.Uploads.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Authorize]
[Route("uploads")]
public class UploadsController : ApiControllerBase
{
    [HttpPost("images")]
    public async Task<IActionResult> UploadImage(
        [FromForm] IFormFile file,
        [FromServices] IImageStorageService imageStorage,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var result = await imageStorage.UploadAsync(
            new ImageUploadRequest(stream, file.FileName, file.ContentType,file.Length),
            cancellationToken);

        return FromResult(result);
    }
}