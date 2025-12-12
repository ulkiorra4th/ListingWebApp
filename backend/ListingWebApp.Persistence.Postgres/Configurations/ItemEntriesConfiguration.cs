using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class ItemEntriesConfiguration : IEntityTypeConfiguration<ItemEntryEntity>
{
    public void Configure(EntityTypeBuilder<ItemEntryEntity> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.HasOne(i => i.Owner)
            .WithMany(a => a.Items);
        
        builder.HasOne(i => i.ItemType)
            .WithMany(i => i.Entries);
        
        builder.Property(i => i.Pseudonym).HasMaxLength(128);

        builder.Property(i => i.CreatedAt).IsRequired();
    }
}