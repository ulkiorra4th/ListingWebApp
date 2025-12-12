using ListingWebApp.Common.Enums;

namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class AccountEntity
{
    public required Guid Id { get; set; }
    
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Salt { get; set; }

    public SessionEntity? Session { get; set; }

    public ICollection<ProfileEntity> Profiles { get; set; } = [];
    public ICollection<ItemEntryEntity> Items { get; set; } = [];
    public ICollection<ListingEntity> Listings { get; set; } = [];
    
    public required AccountStatus Status { get; set; } = AccountStatus.Unverified;
    public required AccountRole Role { get; set; } = AccountRole.User;
    
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}