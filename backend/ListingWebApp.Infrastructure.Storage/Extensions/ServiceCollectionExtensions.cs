using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Infrastructure.Storage.Options;
using ListingWebApp.Infrastructure.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace ListingWebApp.Infrastructure.Storage.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObjectStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration.GetSection("S3Options"));

        services.AddSingleton<IMinioClient>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<S3Options>>().Value;
            return new MinioClient()
                .WithEndpoint(opt.Endpoint)
                .WithCredentials(opt.AccessKey, opt.SecretKey)
                .WithRegion(opt.Region)
                .WithSSL(opt.UseSsl)
                .Build();
        });

        services.AddSingleton<IObjectStorageService, MinioStorageService>();
        return services;
    }
}