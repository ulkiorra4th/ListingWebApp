using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Messages;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Tests;

public sealed class AuthTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private readonly IAuthService _authService;

    public AuthTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _authService = fixture.Provider.GetRequiredService<IAuthService>();
    }

    public Task InitializeAsync()
    {
        _fixture.ResetState();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RegisterAsync_WithValidCredentials_PersistsAccountAndCachesCode()
    {
        var dto = new LoginRequestDto("user@example.com", "Aa1!aaaa");

        var result = await _authService.RegisterAsync(dto);

        Assert.True(result.IsSuccess);

        var accountId = Guid.Parse(result.Value.AccountId);
        Assert.True(_fixture.Database.Accounts.ContainsKey(accountId));

        var cachedSecret = await _fixture.CacheService.GetAsync<VerificationSecretData>(accountId.ToString());
        Assert.True(cachedSecret.IsSuccess);
        Assert.Single(_fixture.VerificationQueue.Messages);
        Assert.Equal(dto.Email, _fixture.VerificationQueue.Messages[0].Email);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidPassword_ReturnsFailure()
    {
        var dto = new LoginRequestDto("weak@example.com", "password");

        var result = await _authService.RegisterAsync(dto);

        Assert.True(result.IsFailed);
        Assert.Empty(_fixture.Database.Accounts);
        Assert.Empty(_fixture.VerificationQueue.Messages);
    }

    [Fact]
    public async Task LoginAsync_WithIncorrectPassword_ReturnsFailure()
    {
        const string email = "login@example.com";
        const string validPassword = "Aa1!aaaa";
        await CreateAccountAsync(email, validPassword);

        var result = await _authService.LoginAsync(email, "Wrong1!");

        Assert.True(result.IsFailed);
        Assert.Empty(_fixture.Database.Sessions);
    }

    [Fact]
    public async Task VerifyAccountAsync_WithValidCode_UpdatesStatus()
    {
        const string email = "verify@example.com";
        const string password = "Aa1!aaaa";
        var account = await CreateAccountAsync(email, password);

        var code = "123456";
        var hashedCode = _fixture.Cryptography.HashSecret(code);
        await _fixture.CacheService.ReplaceAsync(account.Id.ToString(), new VerificationSecretData(hashedCode.Hash, hashedCode.Salt), 10);

        var result = await _authService.VerifyAccountAsync(account.Id, code);

        Assert.True(result.IsSuccess);
        Assert.Equal(AccountStatus.Verified, _fixture.Database.Accounts[account.Id].Status);

        var cacheLookup = await _fixture.CacheService.GetAsync<VerificationSecretData>(account.Id.ToString());
        Assert.True(cacheLookup.IsFailed);
    }

    private async Task<Account> CreateAccountAsync(string email, string password)
    {
        var accountsRepository = _fixture.Provider.GetRequiredService<IAccountsRepository>();
        var crypto = _fixture.Provider.GetRequiredService<ICryptographyService>();
        var hashResult = crypto.HashSecret(password);
        var accountResult = Account.Create(email, hashResult.Hash, hashResult.Salt);

        if (accountResult.IsFailed)
        {
            throw new InvalidOperationException(string.Join(';', accountResult.Errors.Select(e => e.Message)));
        }

        await accountsRepository.CreateAccountAsync(accountResult.Value);
        return accountResult.Value;
    }
}
