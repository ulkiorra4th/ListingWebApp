namespace ListingWebApp.Infrastructure.Storage.Options;

public sealed record S3Options
{
    public required string Endpoint { get; init; } 
    public required string AccessKey { get; init; } 
    public required string SecretKey { get; init; }
    public required string Region { get; init; } = "us-east-1";
    public required string Bucket { get; init; }
    public required bool UseSsl { get; init; }
}