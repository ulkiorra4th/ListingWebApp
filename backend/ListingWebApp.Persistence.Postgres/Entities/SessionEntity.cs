namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class SessionEntity
{
    public required Guid Id { get; set; }
    
    public required Guid AccountId { get; set; }
    public required AccountEntity Account { get; set; }
    
    public required string RefreshTokenHash { get; set; } 
    public required bool IsActive { get; set; } = true;
    
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required DateTime? RevokedAt { get; set; }
}