using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/currencies")]
public sealed class CurrenciesController(ICurrenciesService currenciesService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await currenciesService.GetAllAsync();
        return result.ToActionResult();
    }

    [HttpGet("{code}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByCodeAsync([FromRoute] string code)
    {
        var result = await currenciesService.GetByCodeAsync(code);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCurrencyDto dto)
    {
        var result = await currenciesService.AddAsync(dto);
        return result.ToActionResult(created: true);
    }

    [HttpPatch("{code}/icon")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateIconAsync(
        [FromRoute] string code, 
        [FromForm] IFormFile? file, 
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Icon file is required.");
        }

        await using var stream = file.OpenReadStream();
        var extension = Path.GetExtension(file.FileName);
        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        
        var result = await currenciesService.UpdateIconAsync(code, stream, extension, contentType, ct);
        return result.ToActionResult();
    }
}
