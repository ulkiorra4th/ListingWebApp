using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Abstractions;

public interface IAccountsService
{
    Task<Result<GetAccountDto>> GetAccountByIdAsync(Guid id);
    Task<Result> DeleteAccountAsync(Guid id);
    Task<Result> UpdateStatusAsync(Guid id, AccountStatus status);
}