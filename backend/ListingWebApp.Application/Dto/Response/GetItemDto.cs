using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Response;

public sealed record GetItemDto(
    Guid Id,
    string Name,
    ItemRarity Rarity,
    decimal BasePrice,
    string? Description,
    string? IconKey,
    DateTime ReleaseDate,
    bool IsTrading);
