namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class CurrencyEntity
{
    public required string CurrencyCode { get; set; }
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    public string? IconKey { get; set; }
    
    public decimal? MinTransferAmount { get; set; }
    public decimal? MaxTransferAmount { get; set; }
    
    public required bool IsTransferAllowed { get; set; }
    
}