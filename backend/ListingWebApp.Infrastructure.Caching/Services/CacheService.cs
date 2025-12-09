using FluentResults;
using ListingWebApp.Application.Contracts.Infrastructure;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace ListingWebApp.Infrastructure.Caching.Services;

public sealed class CacheService : ICacheService
{
    private readonly IRedisDatabase _database;

    public CacheService(IRedisDatabase database)
    {
        _database = database;
    }
    
    public async Task<Result<T>> GetAsync<T>(string key)
    {
        var result = await _database.GetAsync<T>(key);
        return result is null 
            ? Result.Fail<T>($"key {key} does not exist")! 
            : Result.Ok(result);
    }

    public async Task<Result<bool>> AddAsync<T>(string key, T value, int expiresInMinutes = 60)
    {
        var resultSuccess = await _database.AddAsync(key, value, expiresIn: TimeSpan.FromMinutes(expiresInMinutes));

        return resultSuccess 
            ? Result.Ok(resultSuccess) 
            : Result.Fail<bool>($"key {key} already exists");
    }

    public async Task<Result<bool>> RemoveAsync(string key)
    {
        var resultSuccess = await _database.RemoveAsync(key);
        
        return resultSuccess 
            ? Result.Ok(resultSuccess) 
            : Result.Fail<bool>($"key {key} does not exist");
    }

    public async Task<Result<bool>> ReplaceAsync<T>(string key, T value, int expiresInMinutes = 60)
    {
        var resultSuccess = 
            await _database.ReplaceAsync(key, value, expiresIn: TimeSpan.FromMinutes(expiresInMinutes));
        
        return resultSuccess 
            ? Result.Ok(resultSuccess) 
            : Result.Fail<bool>($"key {key} does not exist");
    }
}