using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Common.Errors;
using ListingWebApp.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingWebApp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accounts/{accountId:guid}/profiles")]
public sealed class ProfilesController : ControllerBase
{
    private readonly IProfilesService _profilesService;

    public ProfilesController(IProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await _profilesService.GetProfileByIdAsync(accountId, id);
        return result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAllAsync([FromRoute] Guid accountId)
    {
        var result = await _profilesService.GetAllProfilesAsync(accountId);
        return result.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> CreateAsync(
        [FromRoute] Guid accountId,
        [FromBody] CreateProfileDto dto)
    {
        var request = dto with { AccountId = accountId };

        var result = await _profilesService.CreateProfileAsync(request);
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

        var result = await _profilesService.UpdateProfileAsync(request);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await _profilesService.DeleteProfileAsync(id);
        return result.ToActionResult();
    }
}
