using FluentResults;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Listing
{
    public Guid Id { get; }
    public Guid SellerId { get; }
    public Guid ItemEntryId { get; }
    public string CurrencyCode { get; }
    public decimal PriceAmount { get; }
    public ListingStatus Status { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    private Listing(
        Guid id,
        Guid sellerId,
        Guid itemEntryId,
        string currencyCode,
        decimal priceAmount,
        ListingStatus status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        SellerId = sellerId;
        ItemEntryId = itemEntryId;
        CurrencyCode = currencyCode;
        PriceAmount = priceAmount;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<Listing> Create(
        Guid sellerId,
        Guid itemEntryId,
        string currencyCode,
        decimal priceAmount,
        ListingStatus status)
    {
        var now = DateTime.UtcNow;
        return Create(Guid.NewGuid(), sellerId, itemEntryId, currencyCode, priceAmount, status, now, now);
    }

    public static Result<Listing> Create(
        Guid id,
        Guid sellerId,
        Guid itemEntryId,
        string currencyCode,
        decimal priceAmount,
        ListingStatus status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        if (sellerId == Guid.Empty)
            return Result.Fail<Listing>(new ValidationError(nameof(Listing), "SellerId is required."));

        if (itemEntryId == Guid.Empty)
            return Result.Fail<Listing>(new ValidationError(nameof(Listing), "ItemEntryId is required."));

        if (string.IsNullOrWhiteSpace(currencyCode))
            return Result.Fail<Listing>(new ValidationError(nameof(Listing), "CurrencyCode is required."));

        if (priceAmount < 0)
            return Result.Fail<Listing>(new ValidationError(nameof(Listing), "PriceAmount must be non-negative."));

        return Result.Ok(new Listing(
            id: id,
            sellerId: sellerId,
            itemEntryId: itemEntryId,
            currencyCode: currencyCode,
            priceAmount: priceAmount,
            status: status,
            createdAt: createdAt,
            updatedAt: updatedAt));
    }
}
