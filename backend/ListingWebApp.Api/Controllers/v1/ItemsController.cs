using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/items")]
public sealed class ItemsController(IItemsService itemsService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await itemsService.GetByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateItemDto dto)
    {
        var result = await itemsService.CreateAsync(dto);
        return result.ToActionResult(created: true);
    }

    [HttpPatch("{id:guid}/icon")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateIconAsync(
        [FromRoute] Guid id, 
        IFormFile? file, 
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Icon file is required.");
        }
        
        await using var stream = file.OpenReadStream();
        var extension = Path.GetExtension(file.FileName);
        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;

        var result = await itemsService.UpdateIconAsync(id, stream, extension, contentType, ct);
        return result.ToActionResult();
    }
}
