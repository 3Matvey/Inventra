namespace Inventra.Infrastructure.Dropbox;

internal sealed class DropboxOptions
{
    public const string SectionName = "Dropbox";

    public string AccessToken { get; init; } = string.Empty;

    public string SupportTicketsFolder { get; init; } = string.Empty;

    public string UploadUrl { get; init; } = string.Empty;

    public string CreateFolderUrl { get; init; } = string.Empty;
}
