namespace ListingWebApp.Application.Dto.Messages;

public sealed record AccountVerificationMessageDto(
    string Email,
    string VerificationCode
);