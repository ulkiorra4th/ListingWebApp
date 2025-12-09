namespace ListingWebApp.Infrastructure.Email.Models;

public sealed record EmailModel(string To, EmailContent Content);