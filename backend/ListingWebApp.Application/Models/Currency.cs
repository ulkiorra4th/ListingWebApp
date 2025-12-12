using FluentResults;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Currency
{
    public string CurrencyCode { get; }
    public string Name { get; }
    public string? Description { get; }
    public string? IconKey { get; }
    public decimal? MinTransferAmount { get; }
    public decimal? MaxTransferAmount { get; }
    public bool IsTransferAllowed { get; }

    private Currency(
        string currencyCode,
        string name,
        string? description,
        string? iconKey,
        decimal? minTransferAmount,
        decimal? maxTransferAmount,
        bool isTransferAllowed)
    {
        CurrencyCode = currencyCode;
        Name = name;
        Description = description;
        IconKey = iconKey;
        MinTransferAmount = minTransferAmount;
        MaxTransferAmount = maxTransferAmount;
        IsTransferAllowed = isTransferAllowed;
    }

    public static Result<Currency> Create(
        string currencyCode,
        string name,
        string? description,
        string? iconKey,
        decimal? minTransferAmount,
        decimal? maxTransferAmount,
        bool isTransferAllowed)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return Result.Fail<Currency>(new ValidationError(nameof(Currency), "CurrencyCode is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Currency>(new ValidationError(nameof(Currency), "Name is required."));

        if (minTransferAmount is < 0)
            return Result.Fail<Currency>(new ValidationError(nameof(Currency), "MinTransferAmount must be non-negative."));

        if (maxTransferAmount is < 0)
            return Result.Fail<Currency>(new ValidationError(nameof(Currency), "MaxTransferAmount must be non-negative."));

        return Result.Ok(new Currency(
            currencyCode: currencyCode,
            name: name,
            description: description,
            iconKey: iconKey,
            minTransferAmount: minTransferAmount,
            maxTransferAmount: maxTransferAmount,
            isTransferAllowed: isTransferAllowed));
    }
}
