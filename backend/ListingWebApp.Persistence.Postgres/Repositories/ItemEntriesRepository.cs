using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ItemEntriesRepository : IItemEntriesRepository
{
    private readonly PostgresDbContext _context;

    public ItemEntriesRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ItemEntry>> GetByIdAsync(Guid id)
    {
        var entity = await _context.ItemEntries
            .Include(e => e.Owner)
            .Include(e => e.ItemType)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null
            ? Result.Fail<ItemEntry>(new NotFoundError(nameof(ItemEntry)))
            : MapToDomain(entity);
    }

    public async Task<Result<Guid>> CreateAsync(ItemEntry entry)
    {
        var owner = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == entry.OwnerId && a.Status != AccountStatus.Deleted);
        if (owner is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));

        var itemType = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == entry.ItemTypeId);
        if (itemType is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Item)));

        var entity = new ItemEntryEntity
        {
            Id = entry.Id,
            Owner = owner,
            ItemType = itemType,
            Pseudonym = entry.Pseudonym,
            CreatedAt = entry.CreatedAt
        };

        _context.ItemEntries.Add(entity);
        await _context.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }

    private static Result<ItemEntry> MapToDomain(ItemEntryEntity entity) =>
        ItemEntry.Create(
            id: entity.Id,
            ownerId: entity.Owner.Id,
            itemTypeId: entity.ItemType.Id,
            pseudonym: entity.Pseudonym,
            createdAt: entity.CreatedAt);
}
