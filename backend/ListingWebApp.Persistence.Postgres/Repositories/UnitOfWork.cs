using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Persistence.Postgres.Connection;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly PostgresDbContext _context;

    public UnitOfWork(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var result = await action();
        if (result.IsFailed)
        {
            await transaction.RollbackAsync();
            return result;
        }

        await transaction.CommitAsync();
        return Result.Ok();
    }

    public async Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var result = await action();
        if (result.IsFailed)
        {
            await transaction.RollbackAsync();
            return result;
        }

        await transaction.CommitAsync();
        return result;
    }
}
