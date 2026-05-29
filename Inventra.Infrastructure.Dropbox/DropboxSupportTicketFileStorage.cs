using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    public async Task<Result> UploadAsync(string fileName, string content,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateOptions();
        if (!validation.IsSuccess)
            return validation.Error;

        return await UploadToDropboxAsync(fileName, content, cancellationToken);
    }

    private async Task<Result> UploadToDropboxAsync(string fileName, string content,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            if (!token.IsSuccess)
                return token.Error;

            var folderResult = await EnsureFolderAsync(token.Value, cancellationToken);
            if (!folderResult.IsSuccess)
                return folderResult.Error;

            using var request = CreateRequest(fileName, content, token.Value);
            using var response = await httpClient.SendAsync(request, cancellationToken);

            return response.IsSuccessStatusCode ? Result.Success() : UploadFailed();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Dropbox support ticket upload failed.");
            return UploadFailed();
        }
    }

    private async Task<Result<string>> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using var request = CreateTokenRequest();
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return TokenRequestFailed();

        var token = await ReadTokenAsync(response, cancellationToken);
        return string.IsNullOrWhiteSpace(token?.AccessToken) ? TokenRequestFailed() : token.AccessToken;
    }

    private async Task<Result> EnsureFolderAsync(string accessToken, CancellationToken cancellationToken)
    {
        using var request = CreateFolderRequest(accessToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);

        return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Conflict
            ? Result.Success()
            : UploadFailed();
    }

    private HttpRequestMessage CreateTokenRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl);
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.AppKey}:{_options.AppSecret}"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = CreateTokenContent();

        return request;
    }

    private FormUrlEncodedContent CreateTokenContent()
    {
        return new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = _options.RefreshToken
        });
    }

    private static async Task<DropboxTokenResponse?> ReadTokenAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return await response.Content.ReadFromJsonAsync<DropboxTokenResponse>(
            cancellationToken);
    }

    private HttpRequestMessage CreateFolderRequest(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _options.CreateFolderUrl);
        var json = JsonSerializer.Serialize(new { path = NormalizeFolder(), autorename = false });

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return request;
    }

    private HttpRequestMessage CreateRequest(string fileName, string content, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _options.UploadUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

    private Result ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.AppKey))
            return MissingConfiguration(nameof(_options.AppKey));

        if (string.IsNullOrWhiteSpace(_options.AppSecret))
            return MissingConfiguration(nameof(_options.AppSecret));

        return ValidateMoreOptions();
    }

    private Result ValidateMoreOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.RefreshToken))
            return MissingConfiguration(nameof(_options.RefreshToken));

        if (string.IsNullOrWhiteSpace(_options.TokenUrl))
            return MissingConfiguration(nameof(_options.TokenUrl));

        return ValidateStorageOptions();
    }

    private Result ValidateStorageOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.UploadUrl))
            return MissingConfiguration(nameof(_options.UploadUrl));

        if (string.IsNullOrWhiteSpace(_options.CreateFolderUrl))
            return MissingConfiguration(nameof(_options.CreateFolderUrl));

        return string.IsNullOrWhiteSpace(_options.SupportTicketsFolder)
            ? MissingConfiguration(nameof(_options.SupportTicketsFolder))
            : Result.Success();
    }

    private static Error MissingConfiguration(string optionName)
        => Error.Failure(
            "Dropbox.MissingConfiguration",
            $"Dropbox option '{optionName}' is not configured.");

    private static Error TokenRequestFailed()
        => Error.Failure(
            "Dropbox.TokenRequestFailed",
            "Dropbox access token could not be refreshed.");

    private static Error UploadFailed()
        => Error.Failure(
            "Dropbox.UploadFailed",
            "Support ticket could not be uploaded to Dropbox.");

    private sealed record DropboxTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken);
}
