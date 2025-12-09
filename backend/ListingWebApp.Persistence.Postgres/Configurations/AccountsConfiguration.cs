using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class AccountsConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasMany(a => a.Profiles).WithOne(p => p.Account);
        
        builder.Property(a => a.Email)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(a => a.PasswordHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(a => a.Salt)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        
        builder.HasIndex(a => a.Email).IsUnique();
    }
}