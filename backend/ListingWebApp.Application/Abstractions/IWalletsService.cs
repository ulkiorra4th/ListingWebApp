using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface IWalletsService
{
    Task<Result> CreateAsync(CreateWalletDto dto);
    Task<Result<GetWalletDto>> GetByIdAsync(Guid accountId, string currencyCode);
    Task<Result> UpsertAsync(UpsertWalletDto dto);
    Task<Result> CreditAsync(CreditWalletDto dto);
    Task<Result> DebitAsync(DebitWalletDto dto);
}
