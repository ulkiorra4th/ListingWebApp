using FluentResults;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IAccountsRepository
{
    Task<Result<Account>> GetAccountByIdAsync(Guid id);
    Task<Result<Account>> GetAccountByEmailAsync(string email);
    Task<Result<Guid>> CreateAccountAsync(Account account);
    Task<Result<Guid>> AddProfileToAccountAsync(Profile profile);
    Task<Result> DeleteAccountAsync(Guid id);
    Task<Result> UpdateStatusAsync(Guid id, AccountStatus status);
}
