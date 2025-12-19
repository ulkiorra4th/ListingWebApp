using FluentResults;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Common.Errors;
using ListingWebApp.Infrastructure.Storage.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace ListingWebApp.Infrastructure.Storage.Services;

public sealed class MinioStorageService : IObjectStorageService
{
    private readonly IMinioClient _client;
    private readonly string _defaultBucket;
    private readonly ILogger<MinioStorageService> _logger;

    public MinioStorageService(IMinioClient client, IOptions<S3Options> options, ILogger<MinioStorageService> logger)
    {
        _client = client;
        _logger = logger;
        _defaultBucket = options.Value.Bucket;
    }

    public async Task<Result> UploadAsync(string objectName, Stream content, string contentType, CancellationToken ct = default)
    {
        try
        {
            await EnsureBucket(_defaultBucket, ct);
            await _client.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_defaultBucket)
                .WithObject(objectName)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType), ct);
            
            _logger.LogInformation("Object {name} has been uploaded to storage successfully.", objectName);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while uploading object {name} to storage.", objectName);
            return Result.Fail("Error while uploading object.");
        }
    }

    public async Task<Result<Stream>> GetAsync(string objectName, CancellationToken ct = default)
    {
        try
        {
            var ms = new MemoryStream();
            await _client.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_defaultBucket)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(ms)), ct);
            ms.Position = 0;
            return Result.Ok<Stream>(ms);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting object {name} to storage.", objectName);
            return Result.Fail("Error while getting object.");
        }
    }

    public async Task<Result> DeleteAsync(string objectName, CancellationToken ct = default)
    {
        try
        {
            await _client.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(_defaultBucket).WithObject(objectName), ct);
            _logger.LogInformation("Object {name} has been deleted to storage successfully.", objectName);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting object {name} to storage.", objectName);
            return Result.Fail("Error while deleting object.");
        }
        
    }

    public  Result<string> GetDownloadUrl(string path)
    {
        return string.IsNullOrWhiteSpace(path) 
            ? Result.Fail<string>(new ValidationError("Path is required.")) 
            : Path.Combine(_client.Config.BaseUrl.Replace("minio", "localhost"), _defaultBucket, path);
    }

    private async Task EnsureBucket(string bucket, CancellationToken ct)
    {
        try
        {
            var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), ct);
            if (!exists) await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while ensuring bucket {name}.", bucket);
            throw;
        }
        
    }
}