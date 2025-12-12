using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class ItemsConfiguration : IEntityTypeConfiguration<ItemEntity>
{
    public void Configure(EntityTypeBuilder<ItemEntity> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).HasMaxLength(128).IsRequired();
        
        builder.Property(i => i.Description).HasMaxLength(1024);
        builder.Property(i => i.Rarity).IsRequired();
        builder.Property(i => i.BasePrice).IsRequired();
        builder.Property(i => i.IsTrading).IsRequired();
        builder.Property(i => i.ReleaseDate).IsRequired();

        builder.Property(i => i.IconKey).HasMaxLength(128);
    }
}