using System.Text;
using ListingWebApp.Infrastructure.Email.Abstractions;
using ListingWebApp.Infrastructure.Email.Models;

namespace ListingWebApp.Infrastructure.Email.Services;

internal sealed class EmailContentBuilder : IEmailContentBuilder
{
    private readonly string _emailSubject;
    private readonly string _emailBody;

    public char InsertSymbol { get; init; } = '^';

    public EmailContentBuilder(string emailSubject, string emailBody)
    {
        _emailSubject = emailSubject;
        _emailBody = emailBody;
    }

    public EmailContentBuilder(char insertSymbol, string emailSubject, string emailBody) 
        : this(emailSubject, emailBody)
    {
        InsertSymbol = insertSymbol;
    }
    
    public EmailContent BuildMailContent(string verificationToken, bool isBodyHtml = false)
    {
        var body = InsertVerificationTokenToBody(verificationToken, _emailBody);
        return new EmailContent(_emailSubject, body, isBodyHtml);
    }
    
    private string InsertVerificationTokenToBody(string verificationToken, string body)
    {
        var insertSymbolIndex = body.IndexOf(InsertSymbol);
        if (insertSymbolIndex == -1) throw new InvalidDataException("Insert link symbol has not found");
        
        var sb = new StringBuilder(body);
        
        sb.Insert(insertSymbolIndex + 1, $"{verificationToken}");
        sb.Remove(insertSymbolIndex, 1);

        return sb.ToString();
    }
}