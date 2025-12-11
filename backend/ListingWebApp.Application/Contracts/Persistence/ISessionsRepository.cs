using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface ISessionsRepository
{
    Task<Result<Session>> GetSessionByAccountIdAsync(Guid accountId);
    Task<Result<Session>> GetSessionByIdAsync(Guid id);
    Task<Result<Session>> GetSessionByRefreshTokenHashAsync(string refreshTokenHash);
    Task<Result<Guid>> CreateSessionAsync(Account account, string refreshTokenHash, DateTime expiresAt);
    Task<Result<Session>> UpdateSessionAsync(Session session);
    Task<Result> DeleteSessionByAccountIdAsync(Guid accountId);
}
