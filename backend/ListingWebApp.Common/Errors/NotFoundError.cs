using FluentResults;

namespace ListingWebApp.Common.Errors;

public sealed class NotFoundError : Error
{
    public NotFoundError(string failedEntityName) 
        : base($"{failedEntityName} entry not found.") { }
    
    public NotFoundError(string failedEntityName, string metaInfo) 
        : base($"{failedEntityName} entry not found. Context: {metaInfo}") { }
    
    public NotFoundError(
        string failedEntityName,
        string searchfieldName, 
        string searchFieldValue)
        : base($"{failedEntityName} entry with {searchfieldName} = {searchFieldValue} not found.") { }
}