using ListingWebApp.Application.Abstractions;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/v1/trade-transactions")]
public sealed class TradeTransactionsController(ITradeTransactionsService tradeTransactionsService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await tradeTransactionsService.GetByIdAsync(id);
        return result.ToActionResult();
    }
}
