using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class SessionsRepository : ISessionsRepository
{
    private readonly PostgresDbContext _context;

    public SessionsRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Session>> GetSessionByAccountIdAsync(Guid accountId)
    {
        var sessionEntity = await _context.Sessions
            .Include(s => s.Account)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Account.Id == accountId);

        if (sessionEntity is null)
            return Result.Fail<Session>(new NotFoundError(nameof(Session)));

        var sessionResult = MapToDomain(sessionEntity);
        return sessionResult.IsFailed
            ? Result.Fail<Session>(sessionResult.Errors)
            : sessionResult;
    }

    public async Task<Result<Session>> GetSessionByRefreshTokenHashAsync(string refreshTokenHash)
    {
        var sessionEntity = await _context.Sessions
            .Include(s => s.Account)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshTokenHash);

        if (sessionEntity is null)
            return Result.Fail<Session>(new NotFoundError(nameof(Session)));

        var sessionResult = MapToDomain(sessionEntity);
        return sessionResult.IsFailed
            ? Result.Fail<Session>(sessionResult.Errors)
            : sessionResult;
    }

    public async Task<Result<Guid>> CreateSessionAsync(Account account, string refreshTokenHash, DateTime expiresAt)
    {
        var accountEntity = await _context.Accounts
            .Include(a => a.Session)
            .FirstOrDefaultAsync(a => a.Id == account.Id && a.Status != AccountStatus.Deleted);

        if (accountEntity is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));

        var sessionResult = Session.Create(
            accountId: account.Id,
            refreshTokenHash: refreshTokenHash,
            expiresAt: expiresAt);

        if (sessionResult.IsFailed)
            return Result.Fail<Guid>(sessionResult.Errors);

        var session = sessionResult.Value;

        if (accountEntity.Session is not null)
            _context.Sessions.Remove(accountEntity.Session);

        var sessionEntity = new SessionEntity
        {
            Id = session.Id,
            Account = accountEntity,
            RefreshTokenHash = session.RefreshTokenHash,
            IsActive = session.IsActive,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            ExpiresAt = session.ExpiresAt,
            RevokedAt = session.RevokedAt
        };

        _context.Sessions.Add(sessionEntity);
        await _context.SaveChangesAsync();

        return Result.Ok(sessionEntity.Id);
    }

    public async Task<Result<Session>> UpdateSessionAsync(Session session)
    {
        var sessionEntity = await _context.Sessions
            .Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.Id == session.Id && s.Account.Id == session.AccountId);

        if (sessionEntity is null)
            return Result.Fail<Session>(new NotFoundError(nameof(Session)));

        sessionEntity.RefreshTokenHash = session.RefreshTokenHash;
        sessionEntity.IsActive = session.IsActive;
        sessionEntity.UpdatedAt = session.UpdatedAt;
        sessionEntity.ExpiresAt = session.ExpiresAt;
        sessionEntity.RevokedAt = session.RevokedAt;

        await _context.SaveChangesAsync();

        var sessionResult = MapToDomain(sessionEntity);
        return sessionResult.IsFailed
            ? Result.Fail<Session>(sessionResult.Errors)
            : sessionResult;
    }

    public async Task<Result> DeleteSessionByAccountIdAsync(Guid accountId)
    {
        var sessionEntity = await _context.Sessions
            .Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.Account.Id == accountId);

        if (sessionEntity is null)
            return Result.Fail(new NotFoundError(nameof(Session)));

        _context.Sessions.Remove(sessionEntity);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    private static Result<Session> MapToDomain(SessionEntity entity) =>
        Session.Create(
            id: entity.Id,
            accountId: entity.Account.Id,
            refreshTokenHash: entity.RefreshTokenHash,
            isActive: entity.IsActive,
            createdAt: entity.CreatedAt,
            updatedAt: entity.UpdatedAt,
            expiresAt: entity.ExpiresAt,
            revokedAt: entity.RevokedAt);
}
