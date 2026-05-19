using Inventra.Application.Common.Uploads.Dto;

namespace Inventra.Application.Common.Interfaces;

/// <summary>
/// Uploads images to external storage and returns public storage metadata.
/// </summary>
public interface IImageStorageService
{
    Task<Result<UploadedImageDto>> UploadAsync(
        ImageUploadRequest request,
        CancellationToken cancellationToken = default);
}
