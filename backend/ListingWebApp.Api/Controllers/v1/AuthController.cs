using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Api.Dto.Request;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Common.Errors;
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
    public async Task<IActionResult> RegisterAsync([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto.Email, dto.Password);
        return ToActionResult(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequest request)
    {
        var result = await _authService.RefreshAsync(request.UserId, request.RefreshToken);
        return ToActionResult(result);
    }

    [HttpPost("{userId:guid}/logout")]
    public async Task<IActionResult> LogoutAsync([FromRoute] Guid userId)
    {
        var result = await _authService.LogoutAsync(userId);
        return ToActionResult(result);
    }

    [HttpPost("{userId:guid}/verify")]
    public async Task<IActionResult> VerifyAsync(
        [FromRoute] Guid userId,
        [FromBody] VerifyRequestDto dto)
    {
        throw new NotImplementedException();
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
