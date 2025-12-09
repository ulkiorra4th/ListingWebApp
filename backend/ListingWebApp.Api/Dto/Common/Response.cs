namespace ListingWebApp.Api.Dto.Common;

public sealed record Response<T>(string Status, T Data, string? Error = null);