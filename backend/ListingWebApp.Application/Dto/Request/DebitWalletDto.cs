namespace ListingWebApp.Application.Dto.Request;

public sealed record DebitWalletDto(
    Guid AccountId,
    string CurrencyCode,
    decimal Amount,
    DateTime TransactionDate);
