using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/item-entries")]
public sealed class ItemEntriesController : ControllerBase
{
    private readonly IItemEntriesService _itemEntriesService;

    public ItemEntriesController(IItemEntriesService itemEntriesService)
    {
        _itemEntriesService = itemEntriesService;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _itemEntriesService.GetByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateItemEntryDto dto)
    {
        var result = await _itemEntriesService.CreateAsync(dto);
        return result.ToActionResult(created: true);
    }
}
