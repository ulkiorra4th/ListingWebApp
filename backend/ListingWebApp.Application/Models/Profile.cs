using FluentResults;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Profile
{
    public Guid Id { get; }
    public Guid AccountId { get; }

    public string Nickname { get; }
    public int Age { get; }
    public string? IconKey { get; }

    public LanguageCode LanguageCode { get; }
    public CountryCode CountryCode { get; }

    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    private Profile(
        Guid id,
        Guid accountId,
        string nickname,
        int age,
        string? iconKey,
        LanguageCode languageCode,
        CountryCode countryCode,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        AccountId = accountId;
        Nickname = nickname;
        Age = age;
        IconKey = iconKey;
        LanguageCode = languageCode;
        CountryCode = countryCode;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<Profile> Create(
        Guid accountId,
        string nickname,
        int age,
        LanguageCode languageCode,
        CountryCode countryCode)
    {
        if (accountId == Guid.Empty)
            return Result.Fail<Profile>(new ValidationError(nameof(Profile), "AccountId is required."));

        if (string.IsNullOrWhiteSpace(nickname))
            return Result.Fail<Profile>(new ValidationError(nameof(Profile), "Nickname is required."));

        if (age <= 0)
            return Result.Fail<Profile>(new ValidationError(nameof(Profile), "Age must be positive."));

        var now = DateTime.UtcNow;
        return Result.Ok(new Profile(
            id: Guid.NewGuid(),
            accountId: accountId,
            nickname: nickname,
            age: age,
            iconKey: string.Empty,
            languageCode: languageCode,
            countryCode: countryCode,
            createdAt: now,
            updatedAt: now));
    }

    public static Result<Profile> Create(
        Guid id,
        Guid accountId,
        string nickname,
        int age,
        string? iconKey,
        LanguageCode languageCode,
        CountryCode countryCode,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return Result.Ok(new Profile(
            id: id,
            accountId: accountId,
            nickname: nickname,
            age: age,
            iconKey: iconKey,
            languageCode: languageCode,
            countryCode: countryCode,
            createdAt: createdAt,
            updatedAt: updatedAt));
    }
}