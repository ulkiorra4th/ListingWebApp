namespace ListingWebApp.Infrastructure.Email.Options;

public sealed record EmailServiceOptions
{
    public required string VerificationTemplateFilePath { get; init; }
    public required string SenderAddress { get; init; }
    public required string DisplayName { get; init; }
    public required string SenderMailPassword { get; init; }
    public required string SmtpHost { get; init; }
    public required int SmtpPort { get; init; }
}