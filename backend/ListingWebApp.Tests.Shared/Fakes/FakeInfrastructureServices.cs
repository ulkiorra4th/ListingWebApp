using System.Security.Cryptography;
using System.Text;
using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Messages;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Tests.Shared.Fakes;

public sealed class FakeCryptographyService : ICryptographyService
{
    private const int DefaultSaltLength = 16;

    public (string Hash, string Salt) HashSecret(string password)
    {
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(DefaultSaltLength));
        return (ComputeHash(password, salt), salt);
    }

    public bool VerifySecret(string hash, string salt, string password)
        => string.Equals(hash, ComputeHash(password, salt), StringComparison.Ordinal);

    public string ComputeSha256(string token) => ComputeHash(token, string.Empty);

    public string GenerateCode(int codeLength = 6)
    {
        const string digits = "0123456789";
        var buffer = new char[codeLength];

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        }

        return new string(buffer);
    }

    public string GenerateToken(int tokenLength = 64)
    {
        var bytes = RandomNumberGenerator.GetBytes(tokenLength);
        return Convert.ToBase64String(bytes);
    }

    private static string ComputeHash(string value, string salt)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes($"{value}:{salt}");
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}

public sealed class FakeJwtProvider : IJwtProvider
{
    public string GenerateToken(Account account, Guid sessionId)
        => $"token-{account.Id}-{sessionId}";
}

public sealed class InMemoryCacheService : ICacheService
{
    private readonly Dictionary<string, CacheEntry> _cache = new(StringComparer.OrdinalIgnoreCase);

    public Task<Result<T>> GetAsync<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var entry) || entry.IsExpired())
        {
            return Task.FromResult(Result.Fail<T>(new NotFoundError(nameof(key))));
        }

        return Task.FromResult(Result.Ok((T)entry.Value));
    }

    public Task<Result<bool>> AddAsync<T>(string key, T value, int expiresInMinutes = 60)
    {
        _cache[key] = CacheEntry.Create(value!, expiresInMinutes);
        return Task.FromResult(Result.Ok(true));
    }

    public Task<Result<bool>> RemoveAsync(string key)
    {
        var removed = _cache.Remove(key);
        return Task.FromResult(removed ? Result.Ok(true) : Result.Fail<bool>(new NotFoundError(nameof(key))));
    }

    public Task<Result<bool>> ReplaceAsync<T>(string key, T value, int expiresInMinutes = 60)
    {
        _cache[key] = CacheEntry.Create(value!, expiresInMinutes);
        return Task.FromResult(Result.Ok(true));
    }

    public void Clear() => _cache.Clear();

    private sealed record CacheEntry(object Value, DateTimeOffset ExpiresAt)
    {
        public bool IsExpired() => DateTimeOffset.UtcNow > ExpiresAt;

        public static CacheEntry Create(object value, int expiresInMinutes) =>
            new(value, DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes));
    }
}

public sealed class InMemoryObjectStorageService : IObjectStorageService
{
    private readonly Dictionary<string, byte[]> _objects = new(StringComparer.OrdinalIgnoreCase);

    public async Task<Result> UploadAsync(string objectName, Stream content, string contentType, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, ct);
        _objects[objectName] = ms.ToArray();
        return Result.Ok();
    }

    public Task<Result<Stream>> GetAsync(string objectName, CancellationToken ct = default)
    {
        if (!_objects.TryGetValue(objectName, out var data))
        {
            return Task.FromResult(Result.Fail<Stream>(new NotFoundError(nameof(objectName))));
        }

        return Task.FromResult<Result<Stream>>(Result.Ok((Stream)new MemoryStream(data)));
    }

    public Task<Result> DeleteAsync(string objectName, CancellationToken ct = default)
    {
        var removed = _objects.Remove(objectName);
        return Task.FromResult(removed ? Result.Ok() : Result.Fail(new NotFoundError(nameof(objectName))));
    }

    public Result<string> GetDownloadUrl(string path)
    {
        return _objects.ContainsKey(path)
            ? Result.Ok($"https://storage.test/{path}")
            : Result.Fail<string>(new NotFoundError(nameof(path)));
    }

    public IReadOnlyCollection<string> Keys => _objects.Keys.ToList();

    public void Clear() => _objects.Clear();
}

public sealed class FakeAccountVerificationQueue : IAccountVerificationQueue
{
    private readonly List<AccountVerificationMessageDto> _messages = new();

    public IReadOnlyList<AccountVerificationMessageDto> Messages => _messages;

    public ValueTask QueueAsync(AccountVerificationMessageDto message, CancellationToken cancellationToken = default)
    {
        _messages.Add(message);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<AccountVerificationMessageDto> ReadAllAsync(CancellationToken cancellationToken)
    {
        foreach (var message in _messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return message;
            await Task.Yield();
        }
    }

    public void Clear() => _messages.Clear();
}

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action) => action();

    public Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action) => action();
}
