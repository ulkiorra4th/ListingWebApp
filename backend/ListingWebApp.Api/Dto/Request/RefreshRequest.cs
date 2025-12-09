namespace ListingWebApp.Api.Dto.Request;

public sealed record RefreshRequest(Guid UserId, string RefreshToken);
