using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Tests.Shared;
using ListingWebApp.Tests.Shared.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Tests;

public sealed class ApplicationFixture : IDisposable
{
    public ApplicationFixture()
    {
        Provider = Services.CreateProvider();

        Database = Provider.GetRequiredService<InMemoryDatabase>();
        VerificationQueue = (FakeAccountVerificationQueue)Provider.GetRequiredService<IAccountVerificationQueue>();
        CacheService = (InMemoryCacheService)Provider.GetRequiredService<ICacheService>();
        ObjectStorage = (InMemoryObjectStorageService)Provider.GetRequiredService<IObjectStorageService>();
        Cryptography = (FakeCryptographyService)Provider.GetRequiredService<ICryptographyService>();
    }

    public IServiceProvider Provider { get; }
    public InMemoryDatabase Database { get; }
    public FakeAccountVerificationQueue VerificationQueue { get; }
    public InMemoryCacheService CacheService { get; }
    public InMemoryObjectStorageService ObjectStorage { get; }
    public FakeCryptographyService Cryptography { get; }

    public void ResetState()
    {
        Database.Reset();
        VerificationQueue.Clear();
        CacheService.Clear();
        ObjectStorage.Clear();
    }

    public void Dispose()
    {
        if (Provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
