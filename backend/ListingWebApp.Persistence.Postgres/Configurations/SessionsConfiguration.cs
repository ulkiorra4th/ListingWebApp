using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class SessionsConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasOne(s => s.Account).WithOne(a => a.Session);
        
        builder.Property(s => s.RefreshTokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(s => s.IsActive).IsRequired();
        
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();
        builder.Property(s => s.ExpiresAt).IsRequired();
        builder.Property(s => s.RevokedAt);

        builder.HasIndex(s => s.Account).IsUnique();
    }
}