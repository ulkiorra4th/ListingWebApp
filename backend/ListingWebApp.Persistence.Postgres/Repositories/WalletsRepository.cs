using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class WalletsRepository : IWalletsRepository
{
    private readonly PostgresDbContext _context;

    public WalletsRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Wallet>> GetByIdAsync(Guid accountId, string currencyCode)
    {
        var entity = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.AccountId == accountId && w.CurrencyCode == currencyCode);

        return entity is null
            ? Result.Fail<Wallet>(new NotFoundError(nameof(Wallet)))
            : MapToDomain(entity);
    }

    public async Task<Result> UpsertAsync(Wallet wallet)
    {
        var currency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.CurrencyCode == wallet.CurrencyCode);
        if (currency is null)
            return Result.Fail(new NotFoundError(nameof(Currency)));

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == wallet.AccountId && a.Status != AccountStatus.Deleted);
        if (account is null)
            return Result.Fail(new NotFoundError(nameof(Account)));

        var existing = await _context.Wallets
            .Include(w => w.Currency)
            .Include(w => w.Account)
            .FirstOrDefaultAsync(w => w.AccountId == wallet.AccountId && w.CurrencyCode == wallet.CurrencyCode);

        if (existing is null)
        {
            var entity = new WalletEntity
            {
                CurrencyCode = wallet.CurrencyCode,
                AccountId = wallet.AccountId,
                Currency = currency,
                Account = account,
                Balance = wallet.Balance,
                LastTransactionDate = wallet.LastTransactionDate,
                IsActive = wallet.IsActive
            };

            _context.Wallets.Add(entity);
        }
        else
        {
            existing.Balance = wallet.Balance;
            existing.LastTransactionDate = wallet.LastTransactionDate;
            existing.IsActive = wallet.IsActive;
        }

        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateBalanceAsync(Guid accountId, string currencyCode, decimal balance, DateTime? lastTransactionDate)
    {
        var updated = await _context.Wallets
            .Where(w => w.AccountId == accountId && w.CurrencyCode == currencyCode)
            .ExecuteUpdateAsync(u => u
                .SetProperty(w => w.Balance, balance)
                .SetProperty(w => w.LastTransactionDate, lastTransactionDate));

        return updated == 0
            ? Result.Fail(new NotFoundError(nameof(Wallet)))
            : Result.Ok();
    }

    public async Task<Result> IncreaseBalanceAsync(Guid accountId, string currencyCode, decimal amount, DateTime transactionDate)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.AccountId == accountId && w.CurrencyCode == currencyCode);
        if (wallet is null)
            return Result.Fail(new NotFoundError(nameof(Wallet)));

        var updated = await _context.Wallets
            .Where(w => w.AccountId == accountId && w.CurrencyCode == currencyCode)
            .ExecuteUpdateAsync(u => u
                .SetProperty(w => w.Balance, w => w.Balance + amount)
                .SetProperty(w => w.LastTransactionDate, transactionDate));

        return updated == 0
            ? Result.Fail(new NotFoundError(nameof(Wallet)))
            : Result.Ok();
    }

    public async Task<Result> DecreaseBalanceAsync(Guid accountId, string currencyCode, decimal amount, DateTime transactionDate)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.AccountId == accountId && w.CurrencyCode == currencyCode);
        if (wallet is null)
            return Result.Fail(new NotFoundError(nameof(Wallet)));

        if (wallet.Balance < amount)
            return Result.Fail(new ValidationError(nameof(Wallet), "Insufficient funds."));

        var updated = await _context.Wallets
            .Where(w => w.AccountId == accountId && w.CurrencyCode == currencyCode)
            .Where(w => w.Balance >= amount)
            .ExecuteUpdateAsync(u => u
                .SetProperty(w => w.Balance, w => w.Balance - amount)
                .SetProperty(w => w.LastTransactionDate, transactionDate));

        return updated == 0
            ? Result.Fail(new ValidationError(nameof(Wallet), "Insufficient funds."))
            : Result.Ok();
    }

    private static Result<Wallet> MapToDomain(WalletEntity entity) =>
        Wallet.Create(
            currencyCode: entity.CurrencyCode,
            accountId: entity.AccountId,
            balance: entity.Balance,
            lastTransactionDate: entity.LastTransactionDate,
            isActive: entity.IsActive);
}
