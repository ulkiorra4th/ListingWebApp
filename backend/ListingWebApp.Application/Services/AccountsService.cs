using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
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
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> CreateAccountAsync(CreateAccountDto accountDto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteAccountAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateStatusAsync(Guid id, AccountStatus status)
    {
        throw new NotImplementedException();
    }
}