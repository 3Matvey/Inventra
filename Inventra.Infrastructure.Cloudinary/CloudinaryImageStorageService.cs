using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Application.Common.Uploads.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Error = Inventra.Application.Common.Results.Error;

namespace Inventra.Infrastructure.Cloudinary;

internal sealed class CloudinaryImageStorageService(
    IOptions<CloudinaryOptions> options,
    ILogger<CloudinaryImageStorageService> logger) : IImageStorageService
{
    private readonly CloudinaryOptions _options = options.Value;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public async Task<Result<UploadedImageDto>> UploadAsync(ImageUploadRequest request, CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (!validation.IsSuccess)
            return validation.Error;

        return await UploadToCloudinaryAsync(request, cancellationToken);
    }

    private async Task<Result<UploadedImageDto>> UploadToCloudinaryAsync(ImageUploadRequest request, CancellationToken cancellationToken)
    {
        try
        {
            ResetStreamPosition(request.Content);
            var result = await CreateClient().UploadAsync(CreateUploadParams(request), cancellationToken);
            return ToDto(result, request);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Cloudinary image upload failed.");
            return ExternalStorageFailed();
        }
    }

    private Result Validate(ImageUploadRequest request)
    {
        if (request.Size <= 0)
            return EmptyFile();

        if (request.Size > _options.MaxImageSizeBytes)
            return FileTooLarge(_options.MaxImageSizeBytes);

        return IsAllowedContentType(request.ContentType)
            ? Result.Success()
            : UnsupportedContentType(request.ContentType);
    }

    private CloudinaryDotNet.Cloudinary CreateClient()
    {
        var account = new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret);

        return new CloudinaryDotNet.Cloudinary(account);
    }
    private ImageUploadParams CreateUploadParams(ImageUploadRequest request)
    {
        return new ImageUploadParams
        {
            File = new FileDescription(request.FileName, request.Content),
            Folder = _options.UploadFolder,
            PublicId = Guid.CreateVersion7().ToString("N"),
            UseFilename = false,
            UniqueFilename = false,
            Overwrite = false
        };
    }

    private static UploadedImageDto ToDto(ImageUploadResult result, ImageUploadRequest request)
        => new (result.SecureUrl.ToString(), result.PublicId, request.ContentType, request.Size);

    private static void ResetStreamPosition(Stream stream)
    {
        if (stream.CanSeek)
            stream.Position = 0;
    }

    private static bool IsAllowedContentType(string contentType)
    {
        return AllowedContentTypes.Contains(
            contentType,
            StringComparer.OrdinalIgnoreCase);
    }

    private static Error EmptyFile() 
        => Error.BadRequest("Upload.EmptyFile",
            "Uploaded image file is empty.");

    private static Error UnsupportedContentType(string contentType) 
        => Error.BadRequest("Upload.UnsupportedContentType",
            $"Content type '{contentType}' is not supported.");

    private static Error FileTooLarge(long maxSizeBytes) 
        => Error.BadRequest("Upload.FileTooLarge",
            $"Image file size must not exceed {maxSizeBytes} bytes.");
    private static Error ExternalStorageFailed()
        => Error.Failure("Upload.ExternalStorageFailed",
            "Image could not be uploaded to external storage.");
}
