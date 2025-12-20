using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Tests;

public sealed class WalletsServiceTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private readonly IWalletsService _walletsService;

    public WalletsServiceTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _walletsService = fixture.Provider.GetRequiredService<IWalletsService>();
    }

    public Task InitializeAsync()
    {
        _fixture.ResetState();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public async Task CreditAsync_WithNegativeAmount_ReturnsValidationError(decimal amount)
    {
        var dto = new CreditWalletDto(Guid.NewGuid(), "USD", amount, DateTime.UtcNow);

        var result = await _walletsService.CreditAsync(dto);

        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-3.5)]
    public async Task DebitAsync_WithNegativeAmount_ReturnsValidationError(decimal amount)
    {
        var dto = new DebitWalletDto(Guid.NewGuid(), "USD", amount, DateTime.UtcNow);

        var result = await _walletsService.DebitAsync(dto);

        Assert.True(result.IsFailed);
    }
}
