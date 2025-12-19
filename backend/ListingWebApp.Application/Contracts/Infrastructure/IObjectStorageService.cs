using FluentResults;

namespace ListingWebApp.Application.Contracts.Infrastructure;

public interface IObjectStorageService
{
    Task<Result> UploadAsync(string objectName, Stream content, string contentType, CancellationToken ct = default);
    Task<Result<Stream>> GetAsync(string objectName, CancellationToken ct = default);
    Task<Result> DeleteAsync(string objectName, CancellationToken ct = default);
    Result<string> GetDownloadUrl(string path);
}