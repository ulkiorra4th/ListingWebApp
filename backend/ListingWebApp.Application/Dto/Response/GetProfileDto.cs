using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Response;

public sealed record GetProfileDto(
    Guid Id,
    Guid AccountId,
    string Nickname,
    int Age,
    string? IconKey,
    LanguageCode LanguageCode,
    CountryCode CountryCode,
    DateTime CreatedAt,
    DateTime UpdatedAt);
