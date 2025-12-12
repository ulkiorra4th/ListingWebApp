using System.Security.Claims;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/listings/{listingId:guid}")]
public sealed class TradingController : ControllerBase
{
    private readonly ITradingService _tradingService;

    public TradingController(ITradingService tradingService)
    {
        _tradingService = tradingService;
    }

    [HttpPost("purchase")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> PurchaseAsync([FromRoute] Guid listingId)
    {
        var accountId = User.FindFirstValue("accountId");
        if (!Guid.TryParse(accountId, out var accountGuid))
        {
            return Unauthorized();
        }

        var dto = new PurchaseRequestDto(accountGuid, listingId);
        var result = await _tradingService.PurchaseAsync(dto);
        return result.ToActionResult(created: true);
    }
}
