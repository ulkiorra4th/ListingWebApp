using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateProfileDto(
    Guid AccountId,
    string Nickname,
    int Age,
    string? IconKey,
    LanguageCode LanguageCode,
    CountryCode CountryCode);
