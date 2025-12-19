using System;
using System.Linq;
using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Services;

internal sealed class ProfilesService : IProfilesService
{
    private readonly IProfilesRepository _profilesRepository;
    private readonly IObjectStorageService _objectStorageService;

    public ProfilesService(IProfilesRepository profilesRepository, IObjectStorageService objectStorageService)
    {
        _profilesRepository = profilesRepository;
        _objectStorageService = objectStorageService;
    }

    public async Task<Result<GetProfileDto>> GetProfileByIdAsync(Guid accountId, Guid id)
    {
        var profileResult = await _profilesRepository.GetProfileByIdAsync(accountId, id);
        return profileResult.IsFailed 
            ? Result.Fail<GetProfileDto>(profileResult.Errors) 
            : Result.Ok(MapToDto(profileResult.Value));
    }

    public async Task<Result<List<GetProfileDto>>> GetManyProfilesAsync(IEnumerable<Guid> profileIds)
    {
        var profilesResult = await _profilesRepository.GetManyProfilesAsync(profileIds);
        if (profilesResult.IsFailed)
        {
            return Result.Fail<List<GetProfileDto>>(profilesResult.Errors);
        }

        var dtos = profilesResult.Value.Select(MapToDto).ToList();
        return Result.Ok(dtos);
    }

    public async Task<Result<Guid>> CreateProfileAsync(CreateProfileDto profile)
    {
        var profileResult = Profile.Create(
            accountId: profile.AccountId,
            nickname: profile.Nickname,
            age: profile.Age,
            languageCode: profile.LanguageCode,
            countryCode: profile.CountryCode);

        return profileResult.IsFailed
            ? Result.Fail<Guid>(profileResult.Errors)
            : await _profilesRepository.CreateProfileAsync(profileResult.Value);
    }

    public async Task<Result> DeleteProfileAsync(Guid id)
    {
        return await _profilesRepository.DeleteProfileAsync(id);
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileDto profile)
    {
        var existingProfileResult = await _profilesRepository.GetProfileByIdAsync(profile.AccountId, profile.Id);
        if (existingProfileResult.IsFailed)
        {
            return Result.Fail(existingProfileResult.Errors);
        }

        var updatedProfileResult = Profile.Create(
            id: profile.Id,
            accountId: profile.AccountId,
            nickname: profile.Nickname,
            age: profile.Age,
            iconKey: existingProfileResult.Value.IconKey,
            languageCode: profile.LanguageCode,
            countryCode: profile.CountryCode,
            createdAt: existingProfileResult.Value.CreatedAt,
            updatedAt: DateTime.UtcNow);

        return updatedProfileResult.IsFailed
            ? Result.Fail(updatedProfileResult.Errors)
            : await _profilesRepository.UpdateProfileAsync(updatedProfileResult.Value);
    }

    public async Task<Result<List<GetProfileDto>>> GetAllProfilesAsync(Guid accountId)
    {
        var profilesResult = await _profilesRepository.GetAllProfilesAsync(accountId);
        if (profilesResult.IsFailed)
        {
            return Result.Fail<List<GetProfileDto>>(profilesResult.Errors);
        }

        var dtos = profilesResult.Value.Select(MapToDto).ToList();
        return Result.Ok(dtos);
    }

    public async Task<Result> UpdateIconAsync(
        Guid accountId,
        Guid profileId,
        Stream? content,
        string fileExtension,
        string contentType,
        CancellationToken ct = default)
    {
        if (content is null)
        {
            return Result.Fail("Icon content is required.");
        }

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return Result.Fail("File extension is required.");
        }

        var normalizedExtension = fileExtension.StartsWith(".", StringComparison.Ordinal)
            ? fileExtension
            : $".{fileExtension}";

        var iconKey = $"profiles/{profileId}{normalizedExtension}";

        var uploadResult = await _objectStorageService.UploadAsync(iconKey, content, contentType, ct);
        if (uploadResult.IsFailed)
        {
            return Result.Fail(uploadResult.Errors);
        }

        return await _profilesRepository.UpdateIconKeyAsync(accountId, profileId, iconKey);
    }

    public async Task<Result<string>> GetIconUrlAsync(Guid accountId, Guid profileId)
    {
        var profileResult = await _profilesRepository.GetProfileByIdAsync(accountId, profileId);
        if (profileResult.IsFailed)
        {
            return Result.Fail<string>(profileResult.Errors);
        }

        return _objectStorageService.GetDownloadUrl(profileResult.Value.IconKey!);
    }

    private static GetProfileDto MapToDto(Profile profile) =>
        new(
            Id: profile.Id,
            AccountId: profile.AccountId,
            Nickname: profile.Nickname,
            Age: profile.Age,
            IconKey: profile.IconKey,
            LanguageCode: profile.LanguageCode,
            CountryCode: profile.CountryCode,
            CreatedAt: profile.CreatedAt,
            UpdatedAt: profile.UpdatedAt);
}
