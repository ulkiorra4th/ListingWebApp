namespace ListingWebApp.Application.Dto.Request;

public sealed record UpsertWalletDto(
    Guid AccountId,
    string CurrencyCode,
    decimal Balance,
    DateTime? LastTransactionDate,
    bool IsActive);
