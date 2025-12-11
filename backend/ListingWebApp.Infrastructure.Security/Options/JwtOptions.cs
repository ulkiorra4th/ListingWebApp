namespace ListingWebApp.Infrastructure.Security.Options;

public sealed record JwtOptions
{
    public required string SecretKey { get; init; }
    public required int ExpiresMinutes { get; init; }
};