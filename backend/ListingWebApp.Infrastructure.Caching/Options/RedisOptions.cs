namespace ListingWebApp.Infrastructure.Caching.Options;

public sealed record RedisOptions
{
    public string ConnectionString { get; init; } = null!;
    public string KeyPrefix { get; init; } = null!;
    public int Database { get; init; }
}
