using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateListingDto(
    Guid SellerId,
    Guid ItemEntryId,
    string CurrencyCode,
    decimal PriceAmount,
    ListingStatus Status);
