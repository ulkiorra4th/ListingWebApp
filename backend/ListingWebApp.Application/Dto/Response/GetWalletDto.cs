namespace ListingWebApp.Application.Dto.Response;

public sealed record GetWalletDto(
    string CurrencyCode,
    Guid AccountId,
    decimal Balance,
    DateTime? LastTransactionDate,
    bool IsActive);
