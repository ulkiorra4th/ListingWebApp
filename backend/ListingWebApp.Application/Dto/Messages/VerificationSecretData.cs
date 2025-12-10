namespace ListingWebApp.Application.Dto.Messages;

public sealed record VerificationSecretData(string CodeHash, string Salt);