using ListingWebApp.Tests.Shared;

namespace ListingWebApp.Application.Tests;

internal static class Services
{
    public static IServiceProvider CreateProvider() =>
        new ServicesBuilder()
            .AddApplicationServices()
            .AddInMemoryInfrastructure()
            .AddInMemoryRepositories()
            .BuildServiceProvider();
}
