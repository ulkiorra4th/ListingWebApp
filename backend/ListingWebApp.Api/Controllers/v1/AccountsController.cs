using ListingWebApp.Api.Dto.Request;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accounts")]
public sealed class AccountsController(IAccountsService accountsService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await accountsService.GetAccountByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await accountsService.DeleteAccountAsync(id);
        return result.ToActionResult();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatusAsync(
        [FromRoute] Guid id, 
        [FromBody] UpdateAccountStatusRequestDto dto)
    {
        var result = await accountsService.UpdateStatusAsync(id, dto.Status);
        return result.ToActionResult();
    }
}
