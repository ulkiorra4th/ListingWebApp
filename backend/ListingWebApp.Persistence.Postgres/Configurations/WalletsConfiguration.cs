using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class WalletsConfiguration : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.HasKey(w => new { w.CurrencyCode, w.AccountId });

        builder.Property(w => w.CurrencyCode)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(w => w.AccountId).IsRequired();

        builder.Property(w => w.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(w => w.LastTransactionDate);
        builder.Property(w => w.IsActive).IsRequired();

        builder.HasOne(w => w.Currency)
            .WithMany()
            .HasForeignKey(w => w.CurrencyCode);

        builder.HasOne(w => w.Account)
            .WithMany()
            .HasForeignKey(w => w.AccountId);
    }
}
