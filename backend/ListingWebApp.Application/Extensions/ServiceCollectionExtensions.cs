using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountsService, AccountsService>();
        services.AddScoped<IProfilesService, ProfilesService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IAccountVerificationQueue, AccountVerificationQueue>();
        
        return services;
    }
}