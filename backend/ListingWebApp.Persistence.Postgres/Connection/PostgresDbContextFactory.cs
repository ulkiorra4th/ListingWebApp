using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ListingWebApp.Persistence.Postgres.Connection;

public sealed class PostgresDbContextFactory: IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
                 ?? throw new InvalidOperationException("Connection string 'Postgres' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
        optionsBuilder.UseNpgsql(cs);

        return new PostgresDbContext(optionsBuilder.Options);
    }
}