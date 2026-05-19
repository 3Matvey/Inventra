namespace Inventra.Infrastructure.Cloudinary;

internal sealed class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";
    public string CloudName { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string ApiSecret { get; init; } = string.Empty;
    public string UploadFolder { get; init; } = "inventra";
    public long MaxImageSizeBytes { get; init; } = 5 * 1024 * 1024;
}