using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingWebApp.Persistence.Postgres.Repositories;

internal sealed class ProfilesRepository : IProfilesRepository
{
    private readonly PostgresDbContext _context;

    public ProfilesRepository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Profile>> GetProfileByIdAsync(Guid accountId, Guid id)
    {
        var profileEntity = await _context.Profiles
            .Include(p => p.Account)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.Account.Id == accountId);

        return profileEntity is null 
            ? Result.Fail<Profile>(new NotFoundError(nameof(Profile))) 
            : MapToDomain(profileEntity);
    }

    public async Task<Result<List<Profile>>> GetManyProfilesAsync(IEnumerable<Guid> profileIds)
    {
        var ids = profileIds?.ToArray() ?? [];
        
        var profileEntities = await _context.Profiles
            .Include(p => p.Account)
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        var profiles = new List<Profile>(profileEntities.Count);
        
        foreach (var entity in profileEntities)
        {
            var profileResult = MapToDomain(entity);
            if (profileResult.IsFailed)
                return Result.Fail<List<Profile>>(profileResult.Errors);
            
            profiles.Add(profileResult.Value);
        }

        return Result.Ok(profiles);
    }

    public async Task<Result<Guid>> CreateProfileAsync(Profile profile)
    {
        var accountEntity = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == profile.AccountId && a.Status != AccountStatus.Deleted);
        
        if (accountEntity is null)
            return Result.Fail<Guid>(new NotFoundError(nameof(Account)));
        
        var profileEntity = new ProfileEntity
        {
            Id = profile.Id,
            Account = accountEntity,
            Nickname = profile.Nickname,
            Age = profile.Age,
            IconKey = profile.IconKey,
            LanguageCode = profile.LanguageCode,
            CountryCode = profile.CountryCode,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        _context.Profiles.Add(profileEntity);
        await _context.SaveChangesAsync();

        return Result.Ok(profileEntity.Id);
    }

    public async Task<Result> DeleteProfileAsync(Guid id)
    {
        var profileEntity = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
        if (profileEntity is null)
            return Result.Fail(new NotFoundError(nameof(Profile)));

        _context.Profiles.Remove(profileEntity);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> UpdateProfileAsync(Profile profile)
    {
        var profileEntity = await _context.Profiles
            .Include(p => p.Account)
            .FirstOrDefaultAsync(p => p.Id == profile.Id && p.Account.Id == profile.AccountId);

        if (profileEntity is null)
            return Result.Fail(new NotFoundError(nameof(Profile)));

        profileEntity.Nickname = profile.Nickname;
        profileEntity.Age = profile.Age;
        profileEntity.IconKey = profile.IconKey;
        profileEntity.LanguageCode = profile.LanguageCode;
        profileEntity.CountryCode = profile.CountryCode;
        profileEntity.UpdatedAt = profile.UpdatedAt;
        
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<Profile>>> GetAllProfilesAsync(Guid accountId)
    {
        var profileEntities = await _context.Profiles
            .Include(p => p.Account)
            .AsNoTracking()
            .Where(p => p.Account.Id == accountId)
            .ToListAsync();

        var profiles = new List<Profile>(profileEntities.Count);
        
        foreach (var entity in profileEntities)
        {
            var profileResult = MapToDomain(entity);
            if (profileResult.IsFailed)
                return Result.Fail<List<Profile>>(profileResult.Errors);
            
            profiles.Add(profileResult.Value);
        }

        return Result.Ok(profiles);
    }

    private static Result<Profile> MapToDomain(ProfileEntity entity) =>
        Profile.Create(
            id: entity.Id,
            accountId: entity.Account.Id,
            nickname: entity.Nickname,
            age: entity.Age,
            iconKey: entity.IconKey,
            languageCode: entity.LanguageCode,
            countryCode: entity.CountryCode,
            createdAt: entity.CreatedAt,
            updatedAt: entity.UpdatedAt);
}
