using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Infrastructure.Caching.Options;
using ListingWebApp.Infrastructure.Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace ListingWebApp.Infrastructure.Caching.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCachingService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection("RedisOptions"));

        services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;

            return new List<RedisConfiguration>
            {
                new RedisConfiguration
                {
                    ConnectionString = options.ConnectionString,
                    Database = options.Database,
                    KeyPrefix = options.KeyPrefix
                }
            };
        });

        services.AddSingleton<ICacheService, CacheService>();
        
        return services;
    }
}