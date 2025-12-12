namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateTradeTransactionDto(
    Guid BuyerAccountId,
    Guid SellerAccountId,
    Guid ListingId,
    string CurrencyCode,
    decimal Amount,
    bool IsSuspicious,
    DateTime TransactionDate);
