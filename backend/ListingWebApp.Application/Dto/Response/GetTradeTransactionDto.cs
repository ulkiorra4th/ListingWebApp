namespace ListingWebApp.Application.Dto.Response;

public sealed record GetTradeTransactionDto(
    Guid Id,
    Guid BuyerAccountId,
    Guid SellerAccountId,
    Guid ListingId,
    string CurrencyCode,
    decimal Amount,
    bool IsSuspicious,
    DateTime TransactionDate);
