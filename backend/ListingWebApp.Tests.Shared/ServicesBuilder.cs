using ListingWebApp.Tests.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Tests.Shared;

public sealed class ServicesBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    
    public ServicesBuilder()
    {
        _services = new ServiceCollection();
        _configuration = _services.AddConfiguration("testsettings.json");
    }
    
    public ServicesBuilder(string configurationFileName)
    {
        _services = new ServiceCollection();
        _configuration = _services.AddConfiguration(configurationFileName);
    }
    
    public IServiceCollection Build() => _services;


}