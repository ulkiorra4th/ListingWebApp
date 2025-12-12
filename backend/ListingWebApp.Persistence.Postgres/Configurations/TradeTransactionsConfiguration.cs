using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class TradeTransactionsConfiguration : IEntityTypeConfiguration<TradeTransactionEntity>
{
    public void Configure(EntityTypeBuilder<TradeTransactionEntity> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.CurrencyCode)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.IsSuspicious).IsRequired();
        builder.Property(t => t.TransactionDate).IsRequired();

        builder.HasOne(t => t.Currency)
            .WithMany()
            .HasForeignKey(t => t.CurrencyCode);

        builder.Property<Guid>("BuyerAccountId").IsRequired();
        builder.Property<Guid>("SellerAccountId").IsRequired();
        builder.Property<Guid>("ListingId").IsRequired();

        builder.HasOne(t => t.Buyer)
            .WithMany()
            .HasForeignKey("BuyerAccountId");

        builder.HasOne(t => t.Seller)
            .WithMany()
            .HasForeignKey("SellerAccountId");

        builder.HasOne(t => t.Listing)
            .WithMany()
            .HasForeignKey("ListingId");
    }
}
