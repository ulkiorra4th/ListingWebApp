namespace ListingWebApp.Application.Dto.Request;

public sealed record PurchaseRequestDto(
    Guid BuyerAccountId,
    Guid ListingId,
    bool IsSuspicious);
