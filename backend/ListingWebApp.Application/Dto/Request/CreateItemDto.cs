using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateItemDto(
    string Name,
    ItemRarity Rarity,
    decimal BasePrice,
    string? Description,
    string? IconKey,
    DateTime ReleaseDate,
    bool IsTrading);
