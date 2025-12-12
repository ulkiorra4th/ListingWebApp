using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class CurrenciesConfiguration : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
    {
        builder.HasKey(c => c.CurrencyCode);

        builder.Property(c => c.CurrencyCode)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1024);

        builder.Property(c => c.IconKey)
            .HasMaxLength(128);

        builder.Property(c => c.MinTransferAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.MaxTransferAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.IsTransferAllowed).IsRequired();

        builder.HasData(
            new CurrencyEntity
            {
                CurrencyCode = "RUB",
                Name = "Russian Ruble",
                Description = "Base currency for local operations",
                IconKey = "currency-rub",
                MinTransferAmount = 1m,
                MaxTransferAmount = 1_000_000m,
                IsTransferAllowed = true
            },
            new CurrencyEntity
            {
                CurrencyCode = "USD",
                Name = "US Dollar",
                Description = "International settlement currency",
                IconKey = "currency-usd",
                MinTransferAmount = 1m,
                MaxTransferAmount = 1_000_000m,
                IsTransferAllowed = true
            },
            new CurrencyEntity
            {
                CurrencyCode = "EUR",
                Name = "Euro",
                Description = "Eurozone settlement currency",
                IconKey = "currency-eur",
                MinTransferAmount = 1m,
                MaxTransferAmount = 1_000_000m,
                IsTransferAllowed = true
            });
    }
}
