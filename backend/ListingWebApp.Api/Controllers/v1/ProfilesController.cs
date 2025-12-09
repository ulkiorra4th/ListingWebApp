using ListingWebApp.Api.Dto.Common;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Common.Errors;
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
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await _profilesService.GetProfileByIdAsync(accountId, id);
        return ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromRoute] Guid accountId)
    {
        var result = await _profilesService.GetAllProfilesAsync(accountId);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromRoute] Guid accountId,
        [FromBody] CreateProfileDto dto)
    {
        var request = dto with { AccountId = accountId };

        var result = await _profilesService.CreateProfileAsync(request);
        return ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id,
        [FromBody] UpdateProfileDto dto)
    {
        var request = dto with { Id = id, AccountId = accountId };

        var result = await _profilesService.UpdateProfileAsync(request);
        return ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid accountId,
        [FromRoute] Guid id)
    {
        var result = await _profilesService.DeleteProfileAsync(id);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult<T>(FluentResults.Result<T> result)
    {
        if (result.IsSuccess) return Ok(new Response<T>("success", result.Value));

        if (result.Errors.Any(e => e is NotFoundError))
            return NotFound(new Response<string>("error", string.Empty, result.Errors.First().Message));

        return BadRequest(new Response<string>("error", string.Empty,
            string.Join("; ", result.Errors.Select(e => e.Message))));
    }

    private IActionResult ToActionResult(FluentResults.Result result)
    {
        if (result.IsSuccess) return NoContent();

        if (result.Errors.Any(e => e is NotFoundError))
            return NotFound(new Response<string>("error", string.Empty, result.Errors.First().Message));

        return BadRequest(new Response<string>("error", string.Empty,
            string.Join("; ", result.Errors.Select(e => e.Message))));
    }
}