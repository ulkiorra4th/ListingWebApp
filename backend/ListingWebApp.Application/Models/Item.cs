using FluentResults;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Item
{
    public Guid Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public ItemRarity Rarity { get; }
    public decimal BasePrice { get; }
    public string? IconKey { get; }
    public DateTime ReleaseDate { get; }
    public bool IsTrading { get; }

    private Item(
        Guid id,
        string name,
        string? description,
        ItemRarity rarity,
        decimal basePrice,
        string? iconKey,
        DateTime releaseDate,
        bool isTrading)
    {
        Id = id;
        Name = name;
        Description = description;
        Rarity = rarity;
        BasePrice = basePrice;
        IconKey = iconKey;
        ReleaseDate = releaseDate;
        IsTrading = isTrading;
    }

    public static Result<Item> Create(
        string name,
        ItemRarity rarity,
        decimal basePrice,
        string? description,
        string? iconKey,
        DateTime releaseDate,
        bool isTrading)
    {
        return Create(Guid.NewGuid(), name, rarity, basePrice, description, iconKey, releaseDate, isTrading);
    }

    public static Result<Item> Create(
        Guid id,
        string name,
        ItemRarity rarity,
        decimal basePrice,
        string? description,
        string? iconKey,
        DateTime releaseDate,
        bool isTrading)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Item>(new ValidationError(nameof(Item), "Name is required."));

        if (basePrice < 0)
            return Result.Fail<Item>(new ValidationError(nameof(Item), "BasePrice must be non-negative."));

        return Result.Ok(new Item(
            id: id,
            name: name,
            description: description,
            rarity: rarity,
            basePrice: basePrice,
            iconKey: iconKey,
            releaseDate: releaseDate,
            isTrading: isTrading));
    }
}
