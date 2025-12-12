using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ListingsRepository : IListingsRepository
{
    private readonly PostgresDbContext _context;

    public ListingsRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Listing>> GetByIdAsync(Guid id)
    {
        var entity = await _context.Listings
            .Include(l => l.Seller)
            .Include(l => l.Item)
            .Include(l => l.Currency)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        return entity is null
            ? Result.Fail<Listing>(new NotFoundError(nameof(Listing)))
            : MapToDomain(entity);
    }

    public async Task<Result<Guid>> CreateAsync(Listing listing)
    {
        var seller = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == listing.SellerId && a.Status == AccountStatus.Verified);
        if (seller is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));

        var itemEntry = await _context.ItemEntries
            .Include(i => i.ItemType)
            .FirstOrDefaultAsync(i => i.Id == listing.ItemEntryId);
        if (itemEntry is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(ItemEntry)));

        var currency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.CurrencyCode == listing.CurrencyCode);
        if (currency is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Currency)));

        var entity = new ListingEntity
        {
            Id = listing.Id,
            Seller = seller,
            Item = itemEntry,
            Currency = currency,
            PriceAmount = listing.PriceAmount,
            Status = listing.Status,
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt
        };

        _context.Listings.Add(entity);
        await _context.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }

    public async Task<Result> UpdateStatusAsync(Guid id, ListingStatus status)
    {
        var updated = await _context.Listings
            .Where(l => l.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(l => l.Status, status)
                .SetProperty(l => l.UpdatedAt, DateTime.UtcNow));

        return updated == 0
            ? Result.Fail(new NotFoundError(nameof(Listing)))
            : Result.Ok();
    }

    private static Result<Listing> MapToDomain(ListingEntity entity) =>
        Listing.Create(
            id: entity.Id,
            sellerId: entity.Seller.Id,
            itemEntryId: entity.Item.Id,
            currencyCode: entity.Currency.CurrencyCode,
            priceAmount: entity.PriceAmount,
            status: entity.Status,
            createdAt: entity.CreatedAt,
            updatedAt: entity.UpdatedAt);
}
