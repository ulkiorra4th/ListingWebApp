using FluentResults;

namespace ListingWebApp.Infrastructure.Email.Abstractions;

public interface IEmailParser
{
    public string Separator { get; init; }
    public Result<(string Subject, string Body)> Parse(string filePath);
}