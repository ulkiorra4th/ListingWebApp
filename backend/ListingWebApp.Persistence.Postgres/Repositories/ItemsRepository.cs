using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Persistence.Postgres.Connection;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ItemsRepository : IItemsRepository
{
    private readonly PostgresDbContext _context;

    public ItemsRepository(PostgresDbContext context)
    {
        _context = context;
    }
}