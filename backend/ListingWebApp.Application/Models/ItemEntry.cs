using FluentResults;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class ItemEntry
{
    public Guid Id { get; }
    public Guid OwnerId { get; }
    public Guid ItemTypeId { get; }
    public string? Pseudonym { get; }
    public DateTime CreatedAt { get; }

    private ItemEntry(Guid id, Guid ownerId, Guid itemTypeId, string? pseudonym, DateTime createdAt)
    {
        Id = id;
        OwnerId = ownerId;
        ItemTypeId = itemTypeId;
        Pseudonym = pseudonym;
        CreatedAt = createdAt;
    }

    public static Result<ItemEntry> Create(Guid ownerId, Guid itemTypeId, string? pseudonym)
    {
        return Create(Guid.NewGuid(), ownerId, itemTypeId, pseudonym, DateTime.UtcNow);
    }

    public static Result<ItemEntry> Create(Guid id, Guid ownerId, Guid itemTypeId, string? pseudonym, DateTime createdAt)
    {
        if (ownerId == Guid.Empty)
            return Result.Fail<ItemEntry>(new ValidationError(nameof(ItemEntry), "OwnerId is required."));

        if (itemTypeId == Guid.Empty)
            return Result.Fail<ItemEntry>(new ValidationError(nameof(ItemEntry), "ItemTypeId is required."));

        return Result.Ok(new ItemEntry(
            id: id,
            ownerId: ownerId,
            itemTypeId: itemTypeId,
            pseudonym: pseudonym,
            createdAt: createdAt));
    }
}
