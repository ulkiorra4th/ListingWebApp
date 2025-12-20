using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using ListingWebApp.Tests.Shared.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Tests;

public sealed class TradingServiceTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private readonly ITradingService _tradingService;
    private readonly InMemoryDatabase _db;

    public TradingServiceTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _tradingService = fixture.Provider.GetRequiredService<ITradingService>();
        _db = fixture.Database;
    }

    public Task InitializeAsync()
    {
        _fixture.ResetState();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task PurchaseAsync_WithValidData_CompletesTrade()
    {
        var sellerId = await CreateAccountAsync("seller@example.com");
        var buyerId = await CreateAccountAsync("buyer@example.com");
        const string currencyCode = "USD";
        await CreateCurrencyAsync(currencyCode);

        var itemEntryId = await CreateItemEntryAsync(sellerId);
        var listingId = await CreateListingAsync(sellerId, itemEntryId, currencyCode, 50, ListingStatus.Approved);

        await CreateWalletAsync(buyerId, currencyCode, 100);

        var result = await _tradingService.PurchaseAsync(new PurchaseRequestDto(buyerId, listingId));

        Assert.True(result.IsSuccess);
        Assert.Equal(ListingStatus.Closed, _db.Listings[listingId].Status);
        Assert.Equal(buyerId, _db.ItemEntries[itemEntryId].OwnerId);

        var buyerWallet = _db.Wallets[(buyerId, currencyCode.ToUpperInvariant())];
        Assert.Equal(50, buyerWallet.Balance);

        var sellerWallet = _db.Wallets[(sellerId, currencyCode.ToUpperInvariant())];
        Assert.Equal(50, sellerWallet.Balance);

        Assert.Single(_db.TradeTransactions.Values);
        Assert.Equal(listingId, _db.TradeTransactions.Values.First().ListingId);
    }

    [Fact]
    public async Task PurchaseAsync_WhenListingNotApproved_ReturnsValidationError()
    {
        var sellerId = await CreateAccountAsync("seller2@example.com");
        var buyerId = await CreateAccountAsync("buyer2@example.com");
        const string currencyCode = "EUR";
        await CreateCurrencyAsync(currencyCode);

        var itemEntryId = await CreateItemEntryAsync(sellerId);
        var listingId = await CreateListingAsync(sellerId, itemEntryId, currencyCode, 10, ListingStatus.Pending);

        var result = await _tradingService.PurchaseAsync(new PurchaseRequestDto(buyerId, listingId));

        Assert.True(result.IsFailed);
        Assert.IsType<ValidationError>(result.Errors.Single());
        Assert.Empty(_db.TradeTransactions);
    }

    [Fact]
    public async Task PurchaseAsync_WhenBuyerIsSeller_ReturnsValidationError()
    {
        var accountId = await CreateAccountAsync("self@example.com");
        const string currencyCode = "GBP";
        await CreateCurrencyAsync(currencyCode);

        var itemEntryId = await CreateItemEntryAsync(accountId);
        var listingId = await CreateListingAsync(accountId, itemEntryId, currencyCode, 15, ListingStatus.Approved);

        var result = await _tradingService.PurchaseAsync(new PurchaseRequestDto(accountId, listingId));

        Assert.True(result.IsFailed);
        Assert.IsType<ValidationError>(result.Errors.Single());
        Assert.Empty(_db.TradeTransactions);
    }

    private async Task<Guid> CreateAccountAsync(string email)
    {
        var accountsRepository = _fixture.Provider.GetRequiredService<IAccountsRepository>();
        var crypto = _fixture.Provider.GetRequiredService<ICryptographyService>();

        var hash = crypto.HashSecret("Aa1!aaaa");
        var accountResult = Account.Create(email, hash.Hash, hash.Salt);
        if (accountResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', accountResult.Errors.Select(e => e.Message)));
        }

        var createResult = await accountsRepository.CreateAccountAsync(accountResult.Value);
        if (createResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', createResult.Errors.Select(e => e.Message)));
        }

        return createResult.Value;
    }

    private async Task CreateCurrencyAsync(string currencyCode)
    {
        var currenciesRepository = _fixture.Provider.GetRequiredService<ICurrenciesRepository>();
        var currencyResult = Currency.Create(currencyCode, $"{currencyCode} Coin", null, null, 0, 1000, true);

        if (currencyResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', currencyResult.Errors.Select(e => e.Message)));
        }

        var addResult = await currenciesRepository.AddAsync(currencyResult.Value);
        if (addResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', addResult.Errors.Select(e => e.Message)));
        }
    }

    private async Task<Guid> CreateItemEntryAsync(Guid ownerId)
    {
        var itemEntriesRepository = _fixture.Provider.GetRequiredService<IItemEntriesRepository>();
        var itemEntryResult = ItemEntry.Create(ownerId, Guid.NewGuid(), "item");

        if (itemEntryResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', itemEntryResult.Errors.Select(e => e.Message)));
        }

        var createResult = await itemEntriesRepository.CreateAsync(itemEntryResult.Value);
        if (createResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', createResult.Errors.Select(e => e.Message)));
        }

        return createResult.Value;
    }

    private async Task<Guid> CreateListingAsync(Guid sellerId, Guid itemEntryId, string currencyCode, decimal priceAmount, ListingStatus status)
    {
        var listingsRepository = _fixture.Provider.GetRequiredService<IListingsRepository>();
        var listingResult = Listing.Create(sellerId, itemEntryId, currencyCode, priceAmount, status);

        if (listingResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', listingResult.Errors.Select(e => e.Message)));
        }

        var createResult = await listingsRepository.CreateAsync(listingResult.Value);
        if (createResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', createResult.Errors.Select(e => e.Message)));
        }

        return createResult.Value;
    }

    private async Task CreateWalletAsync(Guid accountId, string currencyCode, decimal balance)
    {
        var walletsRepository = _fixture.Provider.GetRequiredService<IWalletsRepository>();
        var walletResult = Wallet.Create(currencyCode, accountId, balance, null, true);

        if (walletResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', walletResult.Errors.Select(e => e.Message)));
        }

        var upsertResult = await walletsRepository.UpsertAsync(walletResult.Value);
        if (upsertResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', upsertResult.Errors.Select(e => e.Message)));
        }
    }
}
