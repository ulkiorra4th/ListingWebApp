using System.Security.Claims;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/v1/accounts/{accountId:guid}/wallets")]
public sealed class WalletsController(IWalletsService walletsService) : ControllerBase
{
    [HttpGet("{currencyCode}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid accountId, 
        [FromRoute] string currencyCode)
    {
        var currentAccountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(currentAccountId, out var accountGuid))
        {
            return Unauthorized();
        }

        if (accountGuid != accountId)
        {
            return Unauthorized("Access denied.");
        }
        
        var result = await walletsService.GetByIdAsync(accountId, currencyCode);
        return result.ToActionResult();
    }

    [HttpPut("{currencyCode}")]
    public async Task<IActionResult> UpsertAsync(
        [FromRoute] Guid accountId, 
        [FromRoute] string currencyCode, 
        [FromBody] UpsertWalletDto dto)
    {
        var currentAccountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(currentAccountId, out var accountGuid))
        {
            return Unauthorized();
        }

        if (accountGuid != accountId)
        {
            return Unauthorized("Access denied.");
        }
        
        var request = dto with { AccountId = accountId, CurrencyCode = currencyCode };
        var result = await walletsService.UpsertAsync(request);
        return result.ToActionResult();
    }

    [HttpPost("{currencyCode}/credit")]
    public async Task<IActionResult> CreditAsync(
        [FromRoute] Guid accountId,
        [FromRoute] string currencyCode,
        [FromBody] CreditWalletDto dto)
    {
        var currentAccountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(currentAccountId, out var accountGuid))
        {
            return Unauthorized();
        }

        if (accountGuid != accountId)
        {
            return Unauthorized("Access denied.");
        }
        
        var request = dto with { AccountId = accountId, CurrencyCode = currencyCode };
        var result = await walletsService.CreditAsync(request);
        return result.ToActionResult();
    }

    [HttpPost("{currencyCode}/debit")]
    public async Task<IActionResult> DebitAsync(
        [FromRoute] Guid accountId, 
        [FromRoute] string currencyCode, 
        [FromBody] DebitWalletDto dto)
    {
        var currentAccountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(currentAccountId, out var accountGuid))
        {
            return Unauthorized();
        }

        if (accountGuid != accountId)
        {
            return Unauthorized("Access denied.");
        }
        
        var request = dto with { AccountId = accountId, CurrencyCode = currencyCode };
        var result = await walletsService.DebitAsync(request);
        return result.ToActionResult();
    }
}
