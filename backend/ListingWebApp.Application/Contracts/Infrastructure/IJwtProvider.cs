using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Infrastructure;

public interface IJwtProvider
{
    string GenerateToken(Account account, Guid sessionId);
}