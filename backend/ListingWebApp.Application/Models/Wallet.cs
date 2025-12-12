using FluentResults;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Wallet
{
    public string CurrencyCode { get; }
    public Guid AccountId { get; }
    public decimal Balance { get; }
    public DateTime? LastTransactionDate { get; }
    public bool IsActive { get; }

    private Wallet(
        string currencyCode,
        Guid accountId,
        decimal balance,
        DateTime? lastTransactionDate,
        bool isActive)
    {
        CurrencyCode = currencyCode;
        AccountId = accountId;
        Balance = balance;
        LastTransactionDate = lastTransactionDate;
        IsActive = isActive;
    }

    public static Result<Wallet> Create(
        string currencyCode,
        Guid accountId,
        decimal balance,
        DateTime? lastTransactionDate,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return Result.Fail<Wallet>(new ValidationError(nameof(Wallet), "CurrencyCode is required."));

        if (accountId == Guid.Empty)
            return Result.Fail<Wallet>(new ValidationError(nameof(Wallet), "AccountId is required."));

        if (balance < 0)
            return Result.Fail<Wallet>(new ValidationError(nameof(Wallet), "Balance must be non-negative."));

        return Result.Ok(new Wallet(
            currencyCode: currencyCode,
            accountId: accountId,
            balance: balance,
            lastTransactionDate: lastTransactionDate,
            isActive: isActive));
    }
}
