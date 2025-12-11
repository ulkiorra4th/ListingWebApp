using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Api.Dto.Request;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Common.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;

    public AccountsController(IAccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _accountsService.GetAccountByIdAsync(id);
        return ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _accountsService.DeleteAccountAsync(id);
        return ToActionResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatusAsync(
        [FromRoute] Guid id, 
        [FromBody] UpdateAccountStatusRequestDto dto)
    {
        var result = await _accountsService.UpdateStatusAsync(id, dto.Status);
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
