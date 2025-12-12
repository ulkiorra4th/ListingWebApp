using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Persistence.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IProfilesRepository, ProfilesRepository>();
        services.AddScoped<ISessionsRepository, SessionsRepository>();
        services.AddScoped<IItemsRepository, ItemsRepository>();
        services.AddScoped<IItemEntriesRepository, ItemEntriesRepository>();
        services.AddScoped<ICurrenciesRepository, CurrenciesRepository>();
        services.AddScoped<IListingsRepository, ListingsRepository>();
        services.AddScoped<ITradeTransactionsRepository, TradeTransactionsRepository>();
        services.AddScoped<IWalletsRepository, WalletsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
