using ListingWebApp.Common.Enums;

namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class ProfileEntity
{
    public required Guid Id { get; set; }
    public required AccountEntity AccountEntity { get; set; }

    public required string Nickname { get; set; }
    public required int Age { get; set; }
    public string? IconKey { get; set; }
    
    public required LanguageCode LanguageCode { get; set; }
    public required CountryCode CountryCode { get; set; }
    
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}