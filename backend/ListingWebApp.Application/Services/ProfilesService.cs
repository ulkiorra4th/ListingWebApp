using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;

namespace ListingWebApp.Application.Services;

internal sealed class ProfilesService : IProfilesService
{
    private readonly IProfilesRepository _profilesRepository;

    public ProfilesService(IProfilesRepository profilesRepository)
    {
        _profilesRepository = profilesRepository;
    }
}