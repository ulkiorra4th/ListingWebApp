namespace ListingWebApp.Application.Dto.Request;

public sealed record CreditWalletDto(
    Guid AccountId,
    string CurrencyCode,
    decimal Amount,
    DateTime TransactionDate);
