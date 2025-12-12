using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface ITradeTransactionsRepository
{
    Task<Result<Guid>> CreateAsync(TradeTransaction transaction);
    Task<Result<TradeTransaction>> GetByIdAsync(Guid id);
}
