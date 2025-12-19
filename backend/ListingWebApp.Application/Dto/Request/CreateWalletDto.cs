namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateWalletDto(
    Guid AccountId,
    string CurrencyCode);
