namespace ListingWebApp.Application.Dto.Response;

public sealed record GetCurrencyDto(
    string CurrencyCode,
    string Name,
    string? Description,
    string? IconKey,
    decimal? MinTransferAmount,
    decimal? MaxTransferAmount,
    bool IsTransferAllowed);
