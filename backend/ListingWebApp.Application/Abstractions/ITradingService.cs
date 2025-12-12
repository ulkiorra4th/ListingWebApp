using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface ITradingService
{
    Task<Result<GetTradeTransactionDto>> PurchaseAsync(PurchaseRequestDto dto);
}
