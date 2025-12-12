namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateCurrencyDto(
    string CurrencyCode,
    string Name,
    string? Description,
    string? IconKey,
    decimal? MinTransferAmount,
    decimal? MaxTransferAmount,
    bool IsTransferAllowed);
