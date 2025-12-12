using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using ListingWebApp.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/listings")]
public sealed class ListingsController : ControllerBase
{
    private readonly IListingsService _listingsService;

    public ListingsController(IListingsService listingsService)
    {
        _listingsService = listingsService;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _listingsService.GetByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateListingDto dto)
    {
        var result = await _listingsService.CreateAsync(dto);
        return result.ToActionResult(created: true);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatusAsync([FromRoute] Guid id, [FromBody] ListingStatus status)
    {
        var result = await _listingsService.UpdateStatusAsync(id, status);
        return result.ToActionResult();
    }
}
