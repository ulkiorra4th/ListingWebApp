using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Dto.Response;

public sealed record GetAccountDto(
    Guid Id,
    string Email,
    AccountStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
