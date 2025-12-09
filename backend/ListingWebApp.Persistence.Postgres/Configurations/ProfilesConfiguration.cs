using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListingWebApp.Persistence.Postgres.Configurations;

public sealed class ProfilesConfiguration : IEntityTypeConfiguration<ProfileEntity>
{
    public void Configure(EntityTypeBuilder<ProfileEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasOne(p => p.AccountEntity).WithMany(a => a.Profiles);
        
        builder.Property(p => p.Nickname)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.IconKey).HasMaxLength(128);
        builder.Property(p => p.Age).IsRequired();
        
        builder.Property(p => p.LanguageCode).IsRequired();
        builder.Property(p => p.CountryCode).IsRequired();
        
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
    }
}