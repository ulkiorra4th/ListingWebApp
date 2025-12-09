using ListingWebApp.Infrastructure.Email.Models;

namespace ListingWebApp.Infrastructure.Email.Abstractions;

public interface IEmailContentBuilder
{
    public char InsertSymbol { get; init; }
    public EmailContent BuildMailContent(string verificationToken, bool isBodyHtml = false);
}