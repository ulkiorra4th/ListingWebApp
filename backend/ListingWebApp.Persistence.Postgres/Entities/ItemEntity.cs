using ListingWebApp.Common.Enums;

namespace ListingWebApp.Persistence.Postgres.Entities;

public sealed class ItemEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required ItemRarity Rarity { get; set; }

    public required decimal BasePrice { get; set; }
    public string? IconKey { get; set; }

    public ICollection<ItemEntryEntity> Entries { get; set; } = [];
    
    public required DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
    public required bool IsTrading { get; set; }
}