namespace ListingWebApp.Infrastructure.Email.Models;

public sealed record EmailContent(string Subject, string Body, bool IsBodyHtml);