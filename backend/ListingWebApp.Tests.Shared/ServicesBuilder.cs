using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Extensions;
using ListingWebApp.Tests.Shared.Extensions;
using ListingWebApp.Tests.Shared.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Tests.Shared;

public sealed class ServicesBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    
    public ServicesBuilder(string configurationFileName = "testsettings.json")
    {
        _services = new ServiceCollection();
        _configuration = _services.AddConfiguration(configurationFileName);
        _services.AddLogging();
    }
    
    public IConfiguration Configuration => _configuration;
    
    public IServiceCollection Services => _services;
    
    public IServiceCollection Build() => _services;

    public IServiceProvider BuildServiceProvider() => _services.BuildServiceProvider();

    public ServicesBuilder AddApplicationServices()
    {
        _services.AddApplication();
        return this;
    }

    public ServicesBuilder AddInMemoryInfrastructure()
    {
        _services.AddSingleton<InMemoryDatabase>();
        _services.AddSingleton<FakeCryptographyService>();
        _services.AddSingleton<ICryptographyService>(sp => sp.GetRequiredService<FakeCryptographyService>());
        _services.AddSingleton<IJwtProvider, FakeJwtProvider>();
        _services.AddSingleton<ICacheService, InMemoryCacheService>();
        _services.AddSingleton<IObjectStorageService, InMemoryObjectStorageService>();
        _services.AddSingleton<IAccountVerificationQueue, FakeAccountVerificationQueue>();
        _services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();
        return this;
    }

    public ServicesBuilder AddInMemoryRepositories()
    {
        _services.AddSingleton<IAccountsRepository, InMemoryAccountsRepository>();
        _services.AddSingleton<IProfilesRepository, InMemoryProfilesRepository>();
        _services.AddSingleton<ISessionsRepository, InMemorySessionsRepository>();
        _services.AddSingleton<IItemsRepository, InMemoryItemsRepository>();
        _services.AddSingleton<IItemEntriesRepository, InMemoryItemEntriesRepository>();
        _services.AddSingleton<ICurrenciesRepository, InMemoryCurrenciesRepository>();
        _services.AddSingleton<IListingsRepository, InMemoryListingsRepository>();
        _services.AddSingleton<ITradeTransactionsRepository, InMemoryTradeTransactionsRepository>();
        _services.AddSingleton<IWalletsRepository, InMemoryWalletsRepository>();
        return this;
    }

}
