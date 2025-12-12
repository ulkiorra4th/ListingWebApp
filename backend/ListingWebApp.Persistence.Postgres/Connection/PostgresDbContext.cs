using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Connection;

public sealed class PostgresDbContext : DbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<ProfileEntity> Profiles { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<ItemEntity> Items { get; set; }
    public DbSet<ItemEntryEntity> ItemEntries { get; set; }
    public DbSet<ListingEntity> Listings { get; set; }
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<TradeTransactionEntity> TradeTransactions { get; set; }
    public DbSet<WalletEntity> Wallets { get; set; }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
        // Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);
}