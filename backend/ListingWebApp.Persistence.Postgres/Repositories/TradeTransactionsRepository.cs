using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class TradeTransactionsRepository : ITradeTransactionsRepository
{
    private readonly PostgresDbContext _context;

    public TradeTransactionsRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> CreateAsync(TradeTransaction transaction)
    {
        var buyer = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == transaction.BuyerAccountId && a.Status == AccountStatus.Verified);
        if (buyer is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));

        var seller = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == transaction.SellerAccountId && a.Status == AccountStatus.Verified);
        if (seller is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));

        var listing = await _context.Listings
            .Include(l => l.Currency)
            .FirstOrDefaultAsync(l => l.Id == transaction.ListingId);
        if (listing is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Listing)));

        var currency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.CurrencyCode == transaction.CurrencyCode);
        if (currency is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Currency)));

        var entity = new TradeTransactionEntity
        {
            Id = transaction.Id,
            CurrencyCode = transaction.CurrencyCode,
            Currency = currency,
            Seller = seller,
            Buyer = buyer,
            Listing = listing,
            Amount = transaction.Amount,
            IsSuspicious = transaction.IsSuspicious,
            TransactionDate = transaction.TransactionDate
        };

        _context.TradeTransactions.Add(entity);
        await _context.SaveChangesAsync();
        return Result.Ok(entity.Id);
    }

    public async Task<Result<TradeTransaction>> GetByIdAsync(Guid id)
    {
        var entity = await _context.TradeTransactions
            .Include(t => t.Currency)
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Listing)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        return entity is null
            ? Result.Fail<TradeTransaction>(new NotFoundError(nameof(TradeTransaction)))
            : MapToDomain(entity);
    }

    private static Result<TradeTransaction> MapToDomain(TradeTransactionEntity entity) =>
        TradeTransaction.Create(
            id: entity.Id,
            buyerAccountId: entity.Buyer.Id,
            sellerAccountId: entity.Seller.Id,
            listingId: entity.Listing.Id,
            currencyCode: entity.CurrencyCode,
            amount: entity.Amount,
            isSuspicious: entity.IsSuspicious,
            transactionDate: entity.TransactionDate);
}
