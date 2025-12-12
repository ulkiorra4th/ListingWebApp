using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface ITradeTransactionsService
{
    Task<Result<GetTradeTransactionDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateTradeTransactionDto dto);
}
