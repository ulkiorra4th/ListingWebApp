using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Tests.Shared.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IConfiguration AddConfiguration(this IServiceCollection services, string fileName)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var parentDirectory = Directory.GetParent(baseDirectory)?.FullName;
        var configDirectory = File.Exists(Path.Combine(baseDirectory, fileName))
            ? baseDirectory
            : parentDirectory ?? baseDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(configDirectory)
            .AddJsonFile(fileName, optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        return configuration;
    }
}
