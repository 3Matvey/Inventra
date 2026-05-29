using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Inventra.Infrastructure.Dropbox;

internal sealed class DropboxSupportTicketFileStorage(
    HttpClient httpClient,
    IOptions<DropboxOptions> options,
    ILogger<DropboxSupportTicketFileStorage> logger) : ISupportTicketFileStorage
{
    private readonly DropboxOptions _options = options.Value;

    public async Task<Result> UploadAsync(
        string fileName,
        string content,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.AccessToken))
            return MissingAccessToken();

        return await UploadToDropboxAsync(fileName, content, cancellationToken);
    }

    private async Task<Result> UploadToDropboxAsync(
        string fileName,
        string content,
        CancellationToken cancellationToken)
    {
        try
        {
            var folderResult = await EnsureFolderAsync(cancellationToken);
            if (!folderResult.IsSuccess)
                return folderResult.Error;

            using var request = CreateRequest(fileName, content);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            return response.IsSuccessStatusCode ? Result.Success() : UploadFailed();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Dropbox support ticket upload failed.");
            return UploadFailed();
        }
    }

    private async Task<Result> EnsureFolderAsync(CancellationToken cancellationToken)
    {
        using var request = CreateFolderRequest();
        using var response = await httpClient.SendAsync(request, cancellationToken);

        return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Conflict
            ? Result.Success()
            : UploadFailed();
    }

    private HttpRequestMessage CreateFolderRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _options.CreateFolderUrl);
        var json = JsonSerializer.Serialize(new { path = NormalizeFolder(), autorename = false });

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return request;
    }

    private HttpRequestMessage CreateRequest(string fileName, string content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _options.UploadUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        request.Headers.Add("Dropbox-API-Arg", CreateApiArg(fileName));
        request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        return request;
    }

    private string CreateApiArg(string fileName)
    {
        return JsonSerializer.Serialize(new
        {
            path = $"{NormalizeFolder()}/{fileName}",
            mode = "add",
            autorename = true,
            mute = false,
            strict_conflict = false
        });
    }

    private string NormalizeFolder()
    {
        var folder = _options.SupportTicketsFolder.Trim();

        return folder.StartsWith('/') ? folder.TrimEnd('/') : $"/{folder.TrimEnd('/')}";
    }

    private static Error MissingAccessToken()
        => Error.Failure(
            "Dropbox.MissingAccessToken",
            "Dropbox access token is not configured.");

    private static Error UploadFailed()
        => Error.Failure(
            "Dropbox.UploadFailed",
            "Support ticket could not be uploaded to Dropbox.");
}
