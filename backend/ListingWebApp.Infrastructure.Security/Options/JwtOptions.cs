namespace ListingWebApp.Infrastructure.Security.Options;

public sealed record JwtOptions(string SecretKey, int ExpiresMinutes);