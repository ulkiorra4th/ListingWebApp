using ListingWebApp.Application.Abstractions;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/trade-transactions")]
public sealed class TradeTransactionsController : ControllerBase
{
    private readonly ITradeTransactionsService _tradeTransactionsService;

    public TradeTransactionsController(ITradeTransactionsService tradeTransactionsService)
    {
        _tradeTransactionsService = tradeTransactionsService;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _tradeTransactionsService.GetByIdAsync(id);
        return result.ToActionResult();
    }
}
