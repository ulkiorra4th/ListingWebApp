using ListingWebApp.Application.Models;

namespace ListingWebApp.Tests.Shared.Fakes;

public sealed class InMemoryDatabase
{
    public Dictionary<Guid, Account> Accounts { get; } = new();
    public Dictionary<string, Guid> AccountIdsByEmail { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<Guid, Session> Sessions { get; } = new();
    public Dictionary<Guid, Profile> Profiles { get; } = new();
    public Dictionary<Guid, Item> Items { get; } = new();
    public Dictionary<Guid, ItemEntry> ItemEntries { get; } = new();
    public Dictionary<string, Currency> Currencies { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<Guid, Listing> Listings { get; } = new();
    public Dictionary<Guid, TradeTransaction> TradeTransactions { get; } = new();
    public Dictionary<(Guid AccountId, string CurrencyCode), Wallet> Wallets { get; } = new();

    public void Reset()
    {
        Accounts.Clear();
        AccountIdsByEmail.Clear();
        Sessions.Clear();
        Profiles.Clear();
        Items.Clear();
        ItemEntries.Clear();
        Currencies.Clear();
        Listings.Clear();
        TradeTransactions.Clear();
        Wallets.Clear();
    }
}
