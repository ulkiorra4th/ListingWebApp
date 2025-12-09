using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ProfilesRepository : IProfilesRepository
{
    public async Task<Result<Profile>> GetProfileByIdAsync(Guid accountId, Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<Profile>>> GetManyProfilesAsync(IEnumerable<Guid> profileIds)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Guid>> CreateProfileAsync(Profile profile)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteProfileAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Profile>> UpdateProfileAsync(Profile profile)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<Profile>>> GetAllProfilesAsync(Guid accountId)
    {
        throw new NotImplementedException();
    }
}