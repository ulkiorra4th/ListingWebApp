using System.Text;
using FluentResults;
using ListingWebApp.Common.Errors;
using ListingWebApp.Infrastructure.Email.Abstractions;

namespace ListingWebApp.Infrastructure.Email.Services;

internal sealed class EmailParser : IEmailParser
{
    private const int SubjectIndex = 0;
    private const int BodyIndex = 1;
    
    public string Separator { get; init; } = "---";
    
    public Result<(string Subject, string Body)> Parse(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return Result.Fail<(string Subject, string Body)>(new NotFoundError("Email template", 
                $"currentPath: {filePath}"));
        }

        if (string.IsNullOrEmpty(Separator))
        {
            return Result
                .Fail<(string Subject, string Body)>(new ValidationError("Separator shouldn't be null or empty"));
        }

        using var sr = new StreamReader(filePath, Encoding.UTF8);
        
        var content = sr.ReadToEnd();
        var parsedContent = content.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        if (parsedContent.Length != 2)
        {
            return Result.Fail<(string Subject, string Body)>(new NotFoundError("Header or body"));
        }

        var subject = parsedContent[SubjectIndex]
            .Replace("\n", String.Empty)
            .Replace("\t", String.Empty)
            .Replace("\r", String.Empty);
        
        var body = parsedContent[BodyIndex];
        return (subject, body);
    }
}