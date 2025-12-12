using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Api.Dto.Request;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Common.Errors;
using ListingWebApp.Api.Extensions;
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
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _accountsService.GetAccountByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _accountsService.DeleteAccountAsync(id);
        return result.ToActionResult();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatusAsync(
        [FromRoute] Guid id, 
        [FromBody] UpdateAccountStatusRequestDto dto)
    {
        var result = await _accountsService.UpdateStatusAsync(id, dto.Status);
        return result.ToActionResult();
    }
}
