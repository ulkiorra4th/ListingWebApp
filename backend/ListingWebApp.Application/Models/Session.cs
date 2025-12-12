using FluentResults;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Session
{
    public Guid Id { get; }
    public Guid AccountId { get; }

    public string RefreshTokenHash { get; }
    public bool IsActive { get; }

    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public DateTime ExpiresAt { get; }
    public DateTime? RevokedAt { get; }

    private Session(
        Guid id,
        Guid accountId,
        string refreshTokenHash,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime expiresAt,
        DateTime? revokedAt)
    {
        Id = id;
        AccountId = accountId;
        RefreshTokenHash = refreshTokenHash;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        ExpiresAt = expiresAt;
        RevokedAt = revokedAt;
    }

    public static Result<Session> Create(
        Guid accountId,
        string refreshTokenHash,
        DateTime expiresAt)
    {
        if (accountId == Guid.Empty)
            return Result.Fail<Session>(new ValidationError(nameof(Session), "AccountId is required."));

        if (string.IsNullOrWhiteSpace(refreshTokenHash))
            return Result.Fail<Session>(new ValidationError(nameof(Session), "Refresh token hash is required."));

        if (expiresAt <= DateTime.UtcNow)
            return Result.Fail<Session>(new ValidationError(nameof(Session), "ExpiresAt must be in the future."));

        var now = DateTime.UtcNow;
        return Result.Ok(new Session(
            id: Guid.NewGuid(),
            accountId: accountId,
            refreshTokenHash: refreshTokenHash,
            isActive: true,
            createdAt: now,
            updatedAt: now,
            expiresAt: expiresAt,
            revokedAt: null));
    }

    public static Result<Session> Create(
        Guid id,
        Guid accountId,
        string refreshTokenHash,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime expiresAt,
        DateTime? revokedAt)
    {
        return Result.Ok(new Session(
            id: id,
            accountId: accountId,
            refreshTokenHash: refreshTokenHash,
            isActive: isActive,
            createdAt: createdAt,
            updatedAt: updatedAt,
            expiresAt: expiresAt,
            revokedAt: revokedAt));
    }
}