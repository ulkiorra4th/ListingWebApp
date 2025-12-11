using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Connection;

internal sealed class PostgresDbContext : DbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<ProfileEntity> Profiles { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);
}