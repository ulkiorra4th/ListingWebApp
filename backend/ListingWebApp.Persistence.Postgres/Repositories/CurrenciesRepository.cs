using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class CurrenciesRepository : ICurrenciesRepository
{
    private readonly PostgresDbContext _context;

    public CurrenciesRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Currency>> GetByCodeAsync(string currencyCode)
    {
        var entity = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CurrencyCode == currencyCode);

        return entity is null
            ? Result.Fail<Currency>(new NotFoundError(nameof(Currency)))
            : MapToDomain(entity);
    }

    public async Task<Result<List<Currency>>> GetAllAsync()
    {
        var entities = await _context.Currencies
            .AsNoTracking()
            .ToListAsync();

        var result = new List<Currency>(entities.Count);
        foreach (var entity in entities)
        {
            var currencyResult = MapToDomain(entity);
            if (currencyResult.IsFailed)
                return Result.Fail<List<Currency>>(currencyResult.Errors);

            result.Add(currencyResult.Value);
        }

        return Result.Ok(result);
    }

    public async Task<Result> AddAsync(Currency currency)
    {
        var exists = await _context.Currencies.AnyAsync(c => c.CurrencyCode == currency.CurrencyCode);
        if (exists)
            return Result.Fail(new ValidationError(nameof(Currency), "Currency already exists."));

        var entity = new CurrencyEntity
        {
            CurrencyCode = currency.CurrencyCode,
            Name = currency.Name,
            Description = currency.Description,
            IconKey = currency.IconKey,
            MinTransferAmount = currency.MinTransferAmount,
            MaxTransferAmount = currency.MaxTransferAmount,
            IsTransferAllowed = currency.IsTransferAllowed
        };

        _context.Currencies.Add(entity);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateIconKeyAsync(string currencyCode, string? iconKey)
    {
        var updated = await _context.Currencies
            .Where(c => c.CurrencyCode == currencyCode)
            .ExecuteUpdateAsync(u => u.SetProperty(c => c.IconKey, iconKey));

        return updated == 0
            ? Result.Fail(new NotFoundError(nameof(Currency)))
            : Result.Ok();
    }

    private static Result<Currency> MapToDomain(CurrencyEntity entity) =>
        Currency.Create(
            currencyCode: entity.CurrencyCode,
            name: entity.Name,
            description: entity.Description,
            iconKey: entity.IconKey,
            minTransferAmount: entity.MinTransferAmount,
            maxTransferAmount: entity.MaxTransferAmount,
            isTransferAllowed: entity.IsTransferAllowed);
}
