using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class ListingsConfiguration : IEntityTypeConfiguration<ListingEntity>
{
    public void Configure(EntityTypeBuilder<ListingEntity> builder)
    {
        builder.HasKey(l => l.Id);
        
        builder.HasOne(l => l.Seller)
            .WithMany(a => a.Listings);

        builder.HasOne(l => l.Item).WithMany();
        builder.HasOne(l => l.Currency).WithMany();
        
        builder.Property(l => l.PriceAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(l => l.Status).IsRequired();
        
        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.UpdatedAt).IsRequired();
    }
}