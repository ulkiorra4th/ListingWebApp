using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface IProfilesService
{
    Task<Result<GetProfileDto>> GetProfileByIdAsync(Guid accountId, Guid id);
    Task<Result<List<GetProfileDto>>> GetManyProfilesAsync(IEnumerable<Guid> profileIds);
    Task<Result<Guid>> CreateProfileAsync(CreateProfileDto profile);
    Task<Result> DeleteProfileAsync(Guid id);
    Task<Result> UpdateProfileAsync(UpdateProfileDto profile);
    Task<Result<List<GetProfileDto>>> GetAllProfilesAsync(Guid accountId);
}
