using FluentResults;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class TradeTransaction
{
    public Guid Id { get; }
    public Guid BuyerAccountId { get; }
    public Guid SellerAccountId { get; }
    public Guid ListingId { get; }
    public string CurrencyCode { get; }
    public decimal Amount { get; }
    public bool IsSuspicious { get; }
    public DateTime TransactionDate { get; }

    private TradeTransaction(
        Guid id,
        Guid buyerAccountId,
        Guid sellerAccountId,
        Guid listingId,
        string currencyCode,
        decimal amount,
        bool isSuspicious,
        DateTime transactionDate)
    {
        Id = id;
        BuyerAccountId = buyerAccountId;
        SellerAccountId = sellerAccountId;
        ListingId = listingId;
        CurrencyCode = currencyCode;
        Amount = amount;
        IsSuspicious = isSuspicious;
        TransactionDate = transactionDate;
    }

    public static Result<TradeTransaction> Create(
        Guid buyerAccountId,
        Guid sellerAccountId,
        Guid listingId,
        string currencyCode,
        decimal amount,
        bool isSuspicious,
        DateTime transactionDate)
    {
        return Create(Guid.NewGuid(), buyerAccountId, sellerAccountId, listingId, currencyCode, amount, isSuspicious, transactionDate);
    }

    public static Result<TradeTransaction> Create(
        Guid id,
        Guid buyerAccountId,
        Guid sellerAccountId,
        Guid listingId,
        string currencyCode,
        decimal amount,
        bool isSuspicious,
        DateTime transactionDate)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return Result.Fail<TradeTransaction>(new ValidationError(nameof(TradeTransaction), "CurrencyCode is required."));

        if (amount < 0)
            return Result.Fail<TradeTransaction>(new ValidationError(nameof(TradeTransaction), "Amount must be non-negative."));

        return Result.Ok(new TradeTransaction(
            id: id,
            buyerAccountId: buyerAccountId,
            sellerAccountId: sellerAccountId,
            listingId: listingId,
            currencyCode: currencyCode,
            amount: amount,
            isSuspicious: isSuspicious,
            transactionDate: transactionDate));
    }
}
