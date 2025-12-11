namespace ListingWebApp.Application.Dto.Response;

public sealed record LoginResponseDto(string AccessToken, string RefreshToken, string AccountId);