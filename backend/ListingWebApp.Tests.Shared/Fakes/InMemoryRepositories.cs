using FluentResults;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Tests.Shared.Fakes;

public sealed class InMemoryAccountsRepository : IAccountsRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryAccountsRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Account>> GetAccountByIdAsync(Guid id)
    {
        return _db.Accounts.TryGetValue(id, out var account)
            ? Task.FromResult(Result.Ok(account))
            : Task.FromResult(Result.Fail<Account>(new NotFoundError(nameof(Account))));
    }

    public Task<Result<Account>> GetAccountByEmailAsync(string email)
    {
        if (_db.AccountIdsByEmail.TryGetValue(email, out var id) && _db.Accounts.TryGetValue(id, out var account))
        {
            return Task.FromResult(Result.Ok(account));
        }

        return Task.FromResult(Result.Fail<Account>(new NotFoundError(nameof(Account))));
    }

    public Task<Result<Guid>> CreateAccountAsync(Account account)
    {
        if (_db.AccountIdsByEmail.ContainsKey(account.Email))
        {
            return Task.FromResult(Result.Fail<Guid>($"Account with email {account.Email} already exists."));
        }

        _db.Accounts[account.Id] = account;
        _db.AccountIdsByEmail[account.Email] = account.Id;
        return Task.FromResult(Result.Ok(account.Id));
    }

    public Task<Result<Guid>> AddProfileToAccountAsync(Profile profile)
    {
        if (!_db.Accounts.ContainsKey(profile.AccountId))
        {
            return Task.FromResult(Result.Fail<Guid>(new NotFoundError(nameof(Account))));
        }

        _db.Profiles[profile.Id] = profile;
        return Task.FromResult(Result.Ok(profile.Id));
    }

    public Task<Result> DeleteAccountAsync(Guid id)
    {
        if (!_db.Accounts.TryGetValue(id, out var account))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Account))));
        }

        _db.Accounts.Remove(id);
        _db.AccountIdsByEmail.Remove(account.Email);

        var sessionsToRemove = _db.Sessions
            .Where(s => s.Value.AccountId == id)
            .Select(s => s.Key)
            .ToList();

        foreach (var sessionId in sessionsToRemove)
        {
            _db.Sessions.Remove(sessionId);
        }

        return Task.FromResult(Result.Ok());
    }

    public Task<Result> UpdateStatusAsync(Guid id, AccountStatus status)
    {
        if (!_db.Accounts.TryGetValue(id, out var account))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Account))));
        }

        var updatedAccountResult = Account.Create(
            id: account.Id,
            email: account.Email,
            passwordHash: account.PasswordHash,
            salt: account.Salt,
            role: account.Role,
            status: status,
            createdAt: account.CreatedAt,
            updatedAt: DateTime.UtcNow);

        if (updatedAccountResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedAccountResult.Errors));
        }

        _db.Accounts[id] = updatedAccountResult.Value;
        _db.AccountIdsByEmail[account.Email] = id;
        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemorySessionsRepository : ISessionsRepository
{
    private readonly InMemoryDatabase _db;

    public InMemorySessionsRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Session>> GetSessionByAccountIdAsync(Guid accountId)
    {
        var session = _db.Sessions.Values.LastOrDefault(s => s.AccountId == accountId);
        return session is null
            ? Task.FromResult(Result.Fail<Session>(new NotFoundError(nameof(Session))))
            : Task.FromResult(Result.Ok(session));
    }

    public Task<Result<Session>> GetSessionByIdAsync(Guid id)
    {
        return _db.Sessions.TryGetValue(id, out var session)
            ? Task.FromResult(Result.Ok(session))
            : Task.FromResult(Result.Fail<Session>(new NotFoundError(nameof(Session))));
    }

    public Task<Result<Session>> GetSessionByRefreshTokenHashAsync(string refreshTokenHash)
    {
        var session = _db.Sessions.Values.LastOrDefault(s => s.RefreshTokenHash == refreshTokenHash);
        return session is null
            ? Task.FromResult(Result.Fail<Session>(new NotFoundError(nameof(Session))))
            : Task.FromResult(Result.Ok(session));
    }

    public Task<Result<Guid>> CreateSessionAsync(Account account, string refreshTokenHash, DateTime expiresAt)
    {
        var sessionResult = Session.Create(account.Id, refreshTokenHash, expiresAt);
        if (sessionResult.IsFailed)
        {
            return Task.FromResult(Result.Fail<Guid>(sessionResult.Errors));
        }

        _db.Sessions[sessionResult.Value.Id] = sessionResult.Value;
        return Task.FromResult(Result.Ok(sessionResult.Value.Id));
    }

    public Task<Result<Session>> UpdateSessionAsync(Session session)
    {
        if (!_db.Sessions.ContainsKey(session.Id))
        {
            return Task.FromResult(Result.Fail<Session>(new NotFoundError(nameof(Session))));
        }

        _db.Sessions[session.Id] = session;
        return Task.FromResult(Result.Ok(session));
    }

    public Task<Result> DeleteSessionByAccountIdAsync(Guid accountId)
    {
        var sessionIds = _db.Sessions
            .Where(s => s.Value.AccountId == accountId)
            .Select(s => s.Key)
            .ToList();

        if (sessionIds.Count == 0)
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Session))));
        }

        foreach (var sessionId in sessionIds)
        {
            _db.Sessions.Remove(sessionId);
        }

        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemoryProfilesRepository : IProfilesRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryProfilesRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Profile>> GetProfileByIdAsync(Guid accountId, Guid id)
    {
        if (_db.Profiles.TryGetValue(id, out var profile) && profile.AccountId == accountId)
        {
            return Task.FromResult(Result.Ok(profile));
        }

        return Task.FromResult(Result.Fail<Profile>(new NotFoundError(nameof(Profile))));
    }

    public Task<Result<List<Profile>>> GetManyProfilesAsync(IEnumerable<Guid> profileIds)
    {
        var ids = profileIds.ToList();
        var profiles = ids
            .Select(id => _db.Profiles.TryGetValue(id, out var profile) ? profile : null)
            .Where(p => p is not null)
            .Cast<Profile>()
            .ToList();

        return profiles.Count == ids.Count
            ? Task.FromResult(Result.Ok(profiles))
            : Task.FromResult(Result.Fail<List<Profile>>(new NotFoundError(nameof(Profile))));
    }

    public Task<Result<Guid>> CreateProfileAsync(Profile profile)
    {
        _db.Profiles[profile.Id] = profile;
        return Task.FromResult(Result.Ok(profile.Id));
    }

    public Task<Result> DeleteProfileAsync(Guid id)
    {
        return _db.Profiles.Remove(id)
            ? Task.FromResult(Result.Ok())
            : Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Profile))));
    }

    public Task<Result> UpdateProfileAsync(Profile profile)
    {
        if (!_db.Profiles.ContainsKey(profile.Id))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Profile))));
        }

        _db.Profiles[profile.Id] = profile;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result<List<Profile>>> GetAllProfilesAsync(Guid accountId)
    {
        var profiles = _db.Profiles.Values.Where(p => p.AccountId == accountId).ToList();
        return Task.FromResult(Result.Ok(profiles));
    }

    public Task<Result> UpdateIconKeyAsync(Guid accountId, Guid profileId, string? iconKey)
    {
        if (!_db.Profiles.TryGetValue(profileId, out var profile) || profile.AccountId != accountId)
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Profile))));
        }

        var updatedProfileResult = Profile.Create(
            id: profile.Id,
            accountId: profile.AccountId,
            nickname: profile.Nickname,
            age: profile.Age,
            iconKey: iconKey,
            languageCode: profile.LanguageCode,
            countryCode: profile.CountryCode,
            createdAt: profile.CreatedAt,
            updatedAt: DateTime.UtcNow);

        if (updatedProfileResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedProfileResult.Errors));
        }

        _db.Profiles[profileId] = updatedProfileResult.Value;
        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemoryItemsRepository : IItemsRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryItemsRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Item>> GetByIdAsync(Guid id)
    {
        return _db.Items.TryGetValue(id, out var item)
            ? Task.FromResult(Result.Ok(item))
            : Task.FromResult(Result.Fail<Item>(new NotFoundError(nameof(Item))));
    }

    public Task<Result<Guid>> CreateAsync(Item item)
    {
        _db.Items[item.Id] = item;
        return Task.FromResult(Result.Ok(item.Id));
    }

    public Task<Result> UpdateIconKeyAsync(Guid id, string? iconKey)
    {
        if (!_db.Items.TryGetValue(id, out var item))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Item))));
        }

        var updatedItemResult = Item.Create(
            id: item.Id,
            name: item.Name,
            rarity: item.Rarity,
            basePrice: item.BasePrice,
            description: item.Description,
            iconKey: iconKey,
            releaseDate: item.ReleaseDate,
            isTrading: item.IsTrading);

        if (updatedItemResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedItemResult.Errors));
        }

        _db.Items[id] = updatedItemResult.Value;
        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemoryItemEntriesRepository : IItemEntriesRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryItemEntriesRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<ItemEntry>> GetByIdAsync(Guid id)
    {
        return _db.ItemEntries.TryGetValue(id, out var entry)
            ? Task.FromResult(Result.Ok(entry))
            : Task.FromResult(Result.Fail<ItemEntry>(new NotFoundError(nameof(ItemEntry))));
    }

    public Task<Result<Guid>> CreateAsync(ItemEntry entry)
    {
        _db.ItemEntries[entry.Id] = entry;
        return Task.FromResult(Result.Ok(entry.Id));
    }

    public Task<Result> TransferOwnershipAsync(Guid entryId, Guid newOwnerId)
    {
        if (!_db.ItemEntries.TryGetValue(entryId, out var entry))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(ItemEntry))));
        }

        var updatedEntryResult = ItemEntry.Create(
            id: entry.Id,
            ownerId: newOwnerId,
            itemTypeId: entry.ItemTypeId,
            pseudonym: entry.Pseudonym,
            createdAt: entry.CreatedAt);

        if (updatedEntryResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedEntryResult.Errors));
        }

        _db.ItemEntries[entryId] = updatedEntryResult.Value;
        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemoryCurrenciesRepository : ICurrenciesRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryCurrenciesRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Currency>> GetByCodeAsync(string currencyCode)
    {
        return _db.Currencies.TryGetValue(currencyCode, out var currency)
            ? Task.FromResult(Result.Ok(currency))
            : Task.FromResult(Result.Fail<Currency>(new NotFoundError(nameof(Currency))));
    }

    public Task<Result<List<Currency>>> GetAllAsync()
    {
        return Task.FromResult(Result.Ok(_db.Currencies.Values.ToList()));
    }

    public Task<Result> AddAsync(Currency currency)
    {
        if (_db.Currencies.ContainsKey(currency.CurrencyCode))
        {
            return Task.FromResult<Result>(Result.Fail(new ValidationError(nameof(Currency), "Currency already exists.")));
        }

        _db.Currencies[currency.CurrencyCode] = currency;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> UpdateIconKeyAsync(string currencyCode, string? iconKey)
    {
        if (!_db.Currencies.TryGetValue(currencyCode, out var currency))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Currency))));
        }

        var updatedCurrencyResult = Currency.Create(
            currencyCode: currency.CurrencyCode,
            name: currency.Name,
            description: currency.Description,
            iconKey: iconKey,
            minTransferAmount: currency.MinTransferAmount,
            maxTransferAmount: currency.MaxTransferAmount,
            isTransferAllowed: currency.IsTransferAllowed);

        if (updatedCurrencyResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedCurrencyResult.Errors));
        }

        _db.Currencies[currencyCode] = updatedCurrencyResult.Value;
        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemoryListingsRepository : IListingsRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryListingsRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Listing>> GetByIdAsync(Guid id)
    {
        return _db.Listings.TryGetValue(id, out var listing)
            ? Task.FromResult(Result.Ok(listing))
            : Task.FromResult(Result.Fail<Listing>(new NotFoundError(nameof(Listing))));
    }

    public Task<Result<Guid>> CreateAsync(Listing listing)
    {
        _db.Listings[listing.Id] = listing;
        return Task.FromResult(Result.Ok(listing.Id));
    }

    public Task<Result> UpdateStatusAsync(Guid id, ListingStatus status)
    {
        if (!_db.Listings.TryGetValue(id, out var listing))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Listing))));
        }

        var updatedListingResult = Listing.Create(
            id: listing.Id,
            sellerId: listing.SellerId,
            itemEntryId: listing.ItemEntryId,
            currencyCode: listing.CurrencyCode,
            priceAmount: listing.PriceAmount,
            status: status,
            createdAt: listing.CreatedAt,
            updatedAt: DateTime.UtcNow);

        if (updatedListingResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedListingResult.Errors));
        }

        _db.Listings[id] = updatedListingResult.Value;
        return Task.FromResult(Result.Ok());
    }
}

public sealed class InMemoryTradeTransactionsRepository : ITradeTransactionsRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryTradeTransactionsRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result<Guid>> CreateAsync(TradeTransaction transaction)
    {
        _db.TradeTransactions[transaction.Id] = transaction;
        return Task.FromResult(Result.Ok(transaction.Id));
    }

    public Task<Result<TradeTransaction>> GetByIdAsync(Guid id)
    {
        return _db.TradeTransactions.TryGetValue(id, out var tx)
            ? Task.FromResult(Result.Ok(tx))
            : Task.FromResult(Result.Fail<TradeTransaction>(new NotFoundError(nameof(TradeTransaction))));
    }
}

public sealed class InMemoryWalletsRepository : IWalletsRepository
{
    private readonly InMemoryDatabase _db;

    public InMemoryWalletsRepository(InMemoryDatabase db)
    {
        _db = db;
    }

    public Task<Result> CreateAsync(Wallet wallet)
    {
        if (!_db.Currencies.ContainsKey(wallet.CurrencyCode))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Currency))));
        }

        if (!_db.Accounts.ContainsKey(wallet.AccountId))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Account))));
        }

        var key = CreateKey(wallet.AccountId, wallet.CurrencyCode);
        if (_db.Wallets.ContainsKey(key))
        {
            return Task.FromResult<Result>(Result.Fail(new ValidationError(nameof(Wallet), "Wallet already exists.")));
        }

        _db.Wallets[key] = wallet;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result<Wallet>> GetByIdAsync(Guid accountId, string currencyCode)
    {
        var key = CreateKey(accountId, currencyCode);
        return _db.Wallets.TryGetValue(key, out var wallet)
            ? Task.FromResult(Result.Ok(wallet))
            : Task.FromResult(Result.Fail<Wallet>(new NotFoundError(nameof(Wallet))));
    }

    public Task<Result> UpsertAsync(Wallet wallet)
    {
        if (!_db.Currencies.ContainsKey(wallet.CurrencyCode))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Currency))));
        }

        if (!_db.Accounts.ContainsKey(wallet.AccountId))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Account))));
        }

        var key = CreateKey(wallet.AccountId, wallet.CurrencyCode);
        _db.Wallets[key] = wallet;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> IncreaseBalanceAsync(Guid accountId, string currencyCode, decimal amount, DateTime transactionDate)
    {
        var key = CreateKey(accountId, currencyCode);
        if (!_db.Wallets.TryGetValue(key, out var wallet))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Wallet))));
        }

        var updatedWalletResult = Wallet.Create(
            currencyCode: wallet.CurrencyCode,
            accountId: wallet.AccountId,
            balance: wallet.Balance + amount,
            lastTransactionDate: transactionDate,
            isActive: wallet.IsActive);

        if (updatedWalletResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedWalletResult.Errors));
        }

        _db.Wallets[key] = updatedWalletResult.Value;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> DecreaseBalanceAsync(Guid accountId, string currencyCode, decimal amount, DateTime transactionDate)
    {
        var key = CreateKey(accountId, currencyCode);
        if (!_db.Wallets.TryGetValue(key, out var wallet))
        {
            return Task.FromResult<Result>(Result.Fail(new NotFoundError(nameof(Wallet))));
        }

        if (wallet.Balance < amount)
        {
            return Task.FromResult<Result>(Result.Fail(new ValidationError(nameof(Wallet), "Insufficient funds.")));
        }

        var updatedWalletResult = Wallet.Create(
            currencyCode: wallet.CurrencyCode,
            accountId: wallet.AccountId,
            balance: wallet.Balance - amount,
            lastTransactionDate: transactionDate,
            isActive: wallet.IsActive);

        if (updatedWalletResult.IsFailed)
        {
            return Task.FromResult(Result.Fail(updatedWalletResult.Errors));
        }

        _db.Wallets[key] = updatedWalletResult.Value;
        return Task.FromResult(Result.Ok());
    }

    private static (Guid, string) CreateKey(Guid accountId, string currencyCode) =>
        (accountId, NormalizeCurrency(currencyCode));

    private static string NormalizeCurrency(string currencyCode) =>
        currencyCode.ToUpperInvariant();
}
