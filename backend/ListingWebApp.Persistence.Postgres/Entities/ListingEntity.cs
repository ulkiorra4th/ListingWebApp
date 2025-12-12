using ListingWebApp.Common.Enums;

namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class ListingEntity
{
    public required Guid Id { get; set; }
    
    public required AccountEntity Seller { get; set; }
    public required ItemEntryEntity Item { get; set; }
    public required CurrencyEntity Currency { get; set; }
    
    public required decimal PriceAmount { get; set; }
    
    public required ListingStatus Status { get; set; } = ListingStatus.Draft;
    
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}