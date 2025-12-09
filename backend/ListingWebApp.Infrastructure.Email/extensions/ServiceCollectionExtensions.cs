using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Infrastructure.Email.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {


        return services;
    }
}