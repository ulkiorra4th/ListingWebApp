using ListingWebApp.Application.Contracts;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Infrastructure.Security.Options;
using ListingWebApp.Infrastructure.Security.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Infrastructure.Security.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

        services.AddSingleton<IHashingService, HashingService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        
        return services;
    }
}