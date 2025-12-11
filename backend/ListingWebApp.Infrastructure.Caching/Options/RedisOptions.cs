namespace ListingWebApp.Infrastructure.Caching.Options;

public sealed record RedisOptions
{
    public required string ConnectionString { get; init; } 
    public required string KeyPrefix { get; init; }
    public required int Database { get; init; }
}
