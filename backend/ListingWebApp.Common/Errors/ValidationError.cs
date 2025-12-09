using FluentResults;

namespace ListingWebApp.Common.Errors;

public sealed class ValidationError : Error
{
    public ValidationError(string failedEntityName, string message)
        : base($"Validation error in {failedEntityName}: {message}." ) { }
    
    public ValidationError(string message)
        : base(message) { }
}