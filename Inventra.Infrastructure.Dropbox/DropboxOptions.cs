namespace Inventra.Infrastructure.Dropbox;

internal sealed class DropboxOptions
{
    public const string SectionName = "Dropbox";

    public string AppKey { get; init; } = string.Empty;

    public string AppSecret { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public string SupportTicketsFolder { get; init; } = string.Empty;

    public string TokenUrl { get; init; } = string.Empty;

    public string UploadUrl { get; init; } = string.Empty;

    public string CreateFolderUrl { get; init; } = string.Empty;
}
