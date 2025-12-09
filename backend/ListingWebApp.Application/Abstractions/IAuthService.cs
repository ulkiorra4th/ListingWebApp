using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(string email, string password);
    Task<Result<LoginResponseDto>> RegisterAsync(LoginRequestDto dto);
    Task<Result> LogoutAsync(Guid userId);
    Task<Result<LoginResponseDto>> RefreshAsync(Guid userId, string refreshToken);
}
