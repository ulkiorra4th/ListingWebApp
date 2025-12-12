using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IWalletsRepository
{
    Task<Result<Wallet>> GetByIdAsync(Guid accountId, string currencyCode);
    Task<Result> UpsertAsync(Wallet wallet);
    Task<Result> IncreaseBalanceAsync(Guid accountId, string currencyCode, decimal amount, DateTime transactionDate);
    Task<Result> DecreaseBalanceAsync(Guid accountId, string currencyCode, decimal amount, DateTime transactionDate);
}
