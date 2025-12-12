namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class WalletEntity
{
    public required string CurrencyCode { get; set; }
    public required Guid AccountId { get; set; }
    
    public required CurrencyEntity Currency { get; set; }
    public required AccountEntity Account { get; set; }
    
    public required decimal Balance { get; set; }
    
    public DateTime? LastTransactionDate { get; set; }
    public required bool IsActive { get; set; } = true;
}