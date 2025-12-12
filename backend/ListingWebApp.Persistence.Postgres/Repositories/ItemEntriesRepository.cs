using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Persistence.Postgres.Connection;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ItemEntriesRepository : IItemEntriesRepository
{
    private readonly PostgresDbContext _context;

    public ItemEntriesRepository(PostgresDbContext context)
    {
        _context = context;
    }
}