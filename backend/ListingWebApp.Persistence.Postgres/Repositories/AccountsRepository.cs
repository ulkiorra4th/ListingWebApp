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

    public async Task<Result<Account>> GetAccountByEmailAsync(string email)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email && x.Status != AccountStatus.Deleted);

        if (account is null) return Result.Fail<Account>(new NotFoundError(nameof(Account)));

        return Account.Create(
            id: account.Id,
            email: account.Email,
            passwordHash: account.PasswordHash,
            salt: account.Salt,
            role: account.Role,
            status: account.Status,
            createdAt: account.CreatedAt,
            updatedAt: account.UpdatedAt);
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
            salt: account.Salt,
            role: account.Role,
            status: account.Status,
            createdAt: account.CreatedAt,
            updatedAt: account.UpdatedAt);
    }

    public async Task<Result<Guid>> CreateAccountAsync(Account account)
    {
        var accountEntity = new AccountEntity
        {
            Id = account.Id,
            Email = account.Email,
            PasswordHash = account.PasswordHash,
            Salt = account.Salt,
            Role = account.Role,
            Status = account.Status,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };

        try
        {
            _context.Accounts.Add(accountEntity);
            await _context.SaveChangesAsync();
            return Result.Ok(accountEntity.Id);
        }
        catch (Exception)
        {
            return Result.Fail<Guid>($"Account with email {account.Email} already exists.");
        }
    }

    public async Task<Result<Guid>> AddProfileToAccountAsync(Profile profile)
    {
        var accountEntity = await _context.Accounts
            .FirstOrDefaultAsync(x => x.Id == profile.AccountId && x.Status != AccountStatus.Deleted);

        if (accountEntity is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));

        var profileEntity = new ProfileEntity
        {
            Id = profile.Id,
            Account = accountEntity,
            Nickname = profile.Nickname,
            Age = profile.Age,
            IconKey = profile.IconKey,
            LanguageCode = profile.LanguageCode,
            CountryCode = profile.CountryCode,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        _context.Profiles.Add(profileEntity);
        await _context.SaveChangesAsync();
        return Result.Ok(profileEntity.Id);
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