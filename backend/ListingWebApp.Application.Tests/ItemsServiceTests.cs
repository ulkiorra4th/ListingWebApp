using System.Text;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Tests.Shared.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Tests;

public sealed class ItemsServiceTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private readonly IItemsService _itemsService;
    private readonly IItemsRepository _itemsRepository;
    private readonly InMemoryObjectStorageService _objectStorage;

    public ItemsServiceTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _itemsService = fixture.Provider.GetRequiredService<IItemsService>();
        _itemsRepository = fixture.Provider.GetRequiredService<IItemsRepository>();
        _objectStorage = (InMemoryObjectStorageService)fixture.Provider.GetRequiredService<IObjectStorageService>();
    }

    public Task InitializeAsync()
    {
        _fixture.ResetState();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UpdateIconAsync_WhenContentIsNull_ReturnsFailure()
    {
        var result = await _itemsService.UpdateIconAsync(Guid.NewGuid(), null, ".png", "image/png");

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task UpdateIconAsync_WithValidData_UpdatesRepositoryAndStorage()
    {
        var itemResult = Item.Create("Item", ItemRarity.Common, 10, "desc", null, DateTime.UtcNow, true);
        await _itemsRepository.CreateAsync(itemResult.Value);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("icon-content"));
        var result = await _itemsService.UpdateIconAsync(itemResult.Value.Id, stream, "png", "image/png");

        Assert.True(result.IsSuccess);

        var updatedItem = await _itemsRepository.GetByIdAsync(itemResult.Value.Id);
        Assert.True(updatedItem.IsSuccess);
        Assert.Equal($"items/{itemResult.Value.Id}.png", updatedItem.Value.IconKey);
        Assert.Contains($"items/{itemResult.Value.Id}.png", _objectStorage.Keys);
    }
}
