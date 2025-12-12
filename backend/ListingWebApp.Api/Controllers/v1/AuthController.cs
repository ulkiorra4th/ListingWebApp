using System.Security.Claims;
using ListingWebApp.Api.Dto.Request;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result.IsFailed)
        {
            return result.ToActionResult();
        }

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return result.ToActionResult();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto.Email, dto.Password);
        if (result.IsFailed)
        {
            return result.ToActionResult();
        }

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshAsync()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(refreshToken);
        return result.ToActionResult();
    }

    [HttpPost("logout")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> LogoutAsync()
    {
        var accountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return Unauthorized();
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Secure = false,
            SameSite = SameSiteMode.None
        });

        var result = await _authService.LogoutAsync(accountGuid);
        return result.ToActionResult();
    }

    [HttpPost("verify")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> VerifyAsync(
        [FromBody] VerifyRequestDto dto)
    {
        var accountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return Unauthorized();
        }

        var result = await _authService.VerifyAccountAsync(accountGuid, dto.Code);
        return result.ToActionResult();
    }
}
