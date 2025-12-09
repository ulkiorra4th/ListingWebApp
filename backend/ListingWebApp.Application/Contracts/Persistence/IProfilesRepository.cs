using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IProfilesRepository
{
    Task<Result<Profile>> GetProfileByIdAsync(Guid accountId, Guid id);
    Task<Result<List<Profile>>> GetManyProfilesAsync(IEnumerable<Guid> profileIds);
    Task<Result<Guid>> CreateProfileAsync(Profile profile);
    Task<Result> DeleteProfileAsync(Guid id);
    Task<Result> UpdateProfileAsync(Profile profile);
    Task<Result<List<Profile>>> GetAllProfilesAsync(Guid accountId);
}