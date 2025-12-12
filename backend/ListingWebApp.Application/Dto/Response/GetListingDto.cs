using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Response;

public sealed record GetListingDto(
    Guid Id,
    Guid SellerId,
    Guid ItemEntryId,
    string CurrencyCode,
    decimal PriceAmount,
    ListingStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
