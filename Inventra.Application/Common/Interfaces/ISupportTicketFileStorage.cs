namespace Inventra.Application.Common.Interfaces;

public interface ISupportTicketFileStorage
{
    Task<Result> UploadAsync(
        string fileName,
        string content,
        CancellationToken cancellationToken = default);
}
