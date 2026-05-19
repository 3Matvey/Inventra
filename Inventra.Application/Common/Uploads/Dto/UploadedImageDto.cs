namespace Inventra.Application.Common.Uploads.Dto;

/// <summary>
/// Describes an image uploaded to external storage.
/// </summary>
public sealed record UploadedImageDto(
    string Url,
    string PublicId,
    string ContentType,
    long Size);
