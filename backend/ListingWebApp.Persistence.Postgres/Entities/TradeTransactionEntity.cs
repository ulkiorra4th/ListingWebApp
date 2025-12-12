namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class TradeTransactionEntity
{
    public required Guid Id { get; set; }
    
    public required string CurrencyCode { get; set; }
    public required CurrencyEntity Currency { get; set; }
    
    public required AccountEntity Seller { get; set; }
    public required AccountEntity Buyer { get; set; }
    
    public required ListingEntity Listing { get; set; }
    
    public required decimal Amount { get; set; }

    public required bool IsSuspicious { get; set; } = false;
    public required DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}