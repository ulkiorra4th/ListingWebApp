using System.Security.Claims;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Authorize(Roles = "User")]
[Route("api/v1/listings/{listingId:guid}")]
public sealed class TradingController(ITradingService tradingService) : ControllerBase
{
    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseAsync([FromRoute] Guid listingId)
    {
        var accountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return Unauthorized();
        }

        var dto = new PurchaseRequestDto(accountGuid, listingId);
        var result = await tradingService.PurchaseAsync(dto);
        return result.ToActionResult(created: true);
    }
}
