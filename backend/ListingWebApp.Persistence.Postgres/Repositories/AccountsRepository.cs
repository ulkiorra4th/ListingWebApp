using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class AccountsRepository : IAccountsRepository
{
    private readonly PostgresDbContext _context;

    public AccountsRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Account>> GetAccountByIdAsync(Guid id)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.Status != AccountStatus.Deleted);
        
        if (account is null) return Result.Fail<Account>(new NotFoundError(nameof(Account)));
        
        return Account.Create(
            id: account.Id,
            email: account.Email,
            passwordHash: account.PasswordHash,
            salt:  account.Salt,
            status: account.Status,
            createdAt: account.CreatedAt,
            updatedAt: account.UpdatedAt);
    }

    public async Task<Result<Guid>> CreateAccountAsync(Account account)
    {
        // TODO: создавать профиль по умолчанию (на уровне бизнес логики)
        
        var accountEntity = new AccountEntity
        {
            Id = account.Id,
            Email = account.Email,
            PasswordHash = account.PasswordHash,
            Salt = account.Salt,
            Status = account.Status,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
        
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteAccountAsync(Guid id)
    {
        var totalUpdated = await _context.Accounts
            .Where(x => x.Id == id && x.Status != AccountStatus.Deleted)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(p => p.Status, AccountStatus.Deleted));
        
        return totalUpdated == 0 
            ? Result.Fail(new NotFoundError(nameof(Account))) 
            : Result.Ok();
    }

    public async Task<Result> UpdateStatusAsync(Guid id, AccountStatus status)
    {
        var totalUpdated = await _context.Accounts
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(p => p.Status, status));
        
        return totalUpdated == 0 
            ? Result.Fail(new NotFoundError(nameof(Account))) 
            : Result.Ok();
    }
}