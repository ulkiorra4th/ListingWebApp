using System.Security.Claims;
using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Api.Dto.Request;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Common.Errors;
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
            return ToActionResult(result);
        }
        
        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        
        return ToActionResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto.Email, dto.Password);
        if (result.IsFailed)
        {
            return ToActionResult(result);
        }
        
        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        
        return ToActionResult(result);
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
        return ToActionResult(result);
    }

    [HttpPost("logout")]
    [Authorize]
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
        return ToActionResult(result);
    }

    [HttpPost("verify")]
    [Authorize]
    public async Task<IActionResult> VerifyAsync(
        [FromBody] VerifyRequestDto dto)
    {
        var accountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return Unauthorized();
        }
        
        var result = await _authService.VerifyAccountAsync(accountGuid, dto.Code);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult<T>(FluentResults.Result<T> result)
    {
        if (result.IsSuccess) return Ok(new Response<T>("success", result.Value));

        if (result.Errors.Any(e => e is NotFoundError))
            return NotFound(new Response<string>("error", string.Empty, result.Errors.First().Message));

        return BadRequest(new Response<string>("error", string.Empty, string.Join("; ", result.Errors.Select(e => e.Message))));
    }

    private IActionResult ToActionResult(FluentResults.Result result)
    {
        if (result.IsSuccess) return NoContent();

        if (result.Errors.Any(e => e is NotFoundError))
            return NotFound(new Response<string>("error", string.Empty, result.Errors.First().Message));

        return BadRequest(new Response<string>("error", string.Empty, string.Join("; ", result.Errors.Select(e => e.Message))));
    }
}
