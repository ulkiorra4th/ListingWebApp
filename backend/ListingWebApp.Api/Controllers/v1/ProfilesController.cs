using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accounts/{accountId:guid}/profiles")]
public sealed class ProfilesController(IProfilesService profilesService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await profilesService.GetProfileByIdAsync(accountId, id);
        return result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAllAsync([FromRoute] Guid accountId)
    {
        var result = await profilesService.GetAllProfilesAsync(accountId);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> CreateAsync(
        [FromRoute] Guid accountId,
        [FromBody] CreateProfileDto dto)
    {
        var request = dto with { AccountId = accountId };

        var result = await profilesService.CreateProfileAsync(request);
        return result.ToActionResult(true);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        [FromBody] UpdateProfileDto dto)
    {
        var request = dto with { Id = id, AccountId = accountId };

        var result = await profilesService.UpdateProfileAsync(request);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await profilesService.DeleteProfileAsync(id);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/icon")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetIconUrlAsync(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await profilesService.GetIconUrlAsync(accountId, id);
        return result.ToActionResult();
    }
    
    [HttpPatch("{id:guid}/icon")]
    [Authorize(Roles = "User,Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateIconAsync(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
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

        var result = await profilesService.UpdateIconAsync(accountId, id, stream, extension, contentType, ct);
        return result.ToActionResult();
    }
}
