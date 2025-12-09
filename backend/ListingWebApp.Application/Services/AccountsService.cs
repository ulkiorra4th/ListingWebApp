using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Services;

internal sealed class AccountsService : IAccountsService
{
    private readonly IAccountsRepository _accountsRepository;

    public AccountsService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<Result<GetAccountDto>> GetAccountByIdAsync(Guid id)
    {
        var accountResult = await _accountsRepository.GetAccountByIdAsync(id);
        if (accountResult.IsFailed)
        {
            return Result.Fail<GetAccountDto>(accountResult.Errors);
        }

        var account = accountResult.Value;
        var dto = new GetAccountDto(
            Id: account.Id,
            Email: account.Email,
            Status: account.Status,
            CreatedAt: account.CreatedAt,
            UpdatedAt: account.UpdatedAt);

        return Result.Ok(dto);
    }

    public async Task<Result> DeleteAccountAsync(Guid id)
        => await _accountsRepository.DeleteAccountAsync(id);


    public async Task<Result> UpdateStatusAsync(Guid id, AccountStatus status)
        => await _accountsRepository.UpdateStatusAsync(id, status);
}