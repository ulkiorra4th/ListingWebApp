namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class ItemEntryEntity
{
    public required Guid Id { get; set; }
    public required AccountEntity Owner { get; set; }
    public required ItemEntity ItemType { get; set; }

    public string? Pseudonym { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}