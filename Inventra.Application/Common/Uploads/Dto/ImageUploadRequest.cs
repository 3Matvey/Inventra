namespace Inventra.Application.Common.Uploads.Dto;

/// <summary>
/// Describes an image file that should be uploaded to external storage.
/// </summary>
public sealed record ImageUploadRequest(
    Stream Content,
    string FileName,
    string ContentType,
    long Size);
