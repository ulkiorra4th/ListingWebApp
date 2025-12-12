using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Services;

internal sealed class WalletsService : IWalletsService
{
    private readonly IWalletsRepository _walletsRepository;

    public WalletsService(IWalletsRepository walletsRepository)
    {
        _walletsRepository = walletsRepository;
    }

    public async Task<Result<GetWalletDto>> GetByIdAsync(Guid accountId, string currencyCode)
    {
        var walletResult = await _walletsRepository.GetByIdAsync(accountId, currencyCode);
        return walletResult.IsFailed
            ? Result.Fail<GetWalletDto>(walletResult.Errors)
            : Result.Ok(MapToDto(walletResult.Value));
    }

    public async Task<Result> UpsertAsync(UpsertWalletDto dto)
    {
        var walletResult = Wallet.Create(
            currencyCode: dto.CurrencyCode,
            accountId: dto.AccountId,
            balance: dto.Balance,
            lastTransactionDate: dto.LastTransactionDate,
            isActive: dto.IsActive);

        return walletResult.IsFailed
            ? Result.Fail(walletResult.Errors)
            : await _walletsRepository.UpsertAsync(walletResult.Value);
    }

    public async Task<Result> CreditAsync(CreditWalletDto dto)
    {
        if (dto.Amount < 0)
            return Result.Fail(new ValidationError(nameof(Wallet), "Amount must be positive."));

        return await _walletsRepository.IncreaseBalanceAsync(dto.AccountId, dto.CurrencyCode, dto.Amount, dto.TransactionDate);
    }

    public async Task<Result> DebitAsync(DebitWalletDto dto)
    {
        if (dto.Amount < 0)
            return Result.Fail(new ValidationError(nameof(Wallet), "Amount must be positive."));

        return await _walletsRepository.DecreaseBalanceAsync(dto.AccountId, dto.CurrencyCode, dto.Amount, dto.TransactionDate);
    }

    private static GetWalletDto MapToDto(Wallet wallet) =>
        new(
            CurrencyCode: wallet.CurrencyCode,
            AccountId: wallet.AccountId,
            Balance: wallet.Balance,
            LastTransactionDate: wallet.LastTransactionDate,
            IsActive: wallet.IsActive);
}
