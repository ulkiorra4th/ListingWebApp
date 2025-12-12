namespace ListingWebApp.Application.Dto.Request;

public sealed record CreateItemEntryDto(
    Guid OwnerId,
    Guid ItemTypeId,
    string? Pseudonym);
