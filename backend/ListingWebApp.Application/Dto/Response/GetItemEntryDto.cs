namespace ListingWebApp.Application.Dto.Response;

public sealed record GetItemEntryDto(
    Guid Id,
    Guid OwnerId,
    Guid ItemTypeId,
    string? Pseudonym,
    DateTime CreatedAt);
