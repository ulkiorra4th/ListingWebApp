using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ItemsRepository : IItemsRepository
{
    private readonly PostgresDbContext _context;

    public ItemsRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Item>> GetByIdAsync(Guid id)
    {
        var entity = await _context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return entity is null
            ? Result.Fail<Item>(new NotFoundError(nameof(Item)))
            : MapToDomain(entity);
    }

    public async Task<Result<Guid>> CreateAsync(Item item)
    {
        var entity = new ItemEntity
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Rarity = item.Rarity,
            BasePrice = item.BasePrice,
            IconKey = item.IconKey,
            ReleaseDate = item.ReleaseDate,
            IsTrading = item.IsTrading
        };

        _context.Items.Add(entity);
        await _context.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }

    public async Task<Result> UpdateIconKeyAsync(Guid id, string? iconKey)
    {
        var updated = await _context.Items
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(i => i.IconKey, iconKey));

        return updated == 0
            ? Result.Fail(new NotFoundError(nameof(Item)))
            : Result.Ok();
    }

    private static Result<Item> MapToDomain(ItemEntity entity) =>
        Item.Create(
            id: entity.Id,
            name: entity.Name,
            rarity: entity.Rarity,
            basePrice: entity.BasePrice,
            description: entity.Description,
            iconKey: entity.IconKey,
            releaseDate: entity.ReleaseDate,
            isTrading: entity.IsTrading);
}
