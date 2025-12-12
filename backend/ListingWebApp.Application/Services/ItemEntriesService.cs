using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Services;

internal sealed class ItemEntriesService : IItemEntriesService
{
    private readonly IItemEntriesRepository _itemEntriesRepository;

    public ItemEntriesService(IItemEntriesRepository itemEntriesRepository)
    {
        _itemEntriesRepository = itemEntriesRepository;
    }

    public async Task<Result<GetItemEntryDto>> GetByIdAsync(Guid id)
    {
        var entryResult = await _itemEntriesRepository.GetByIdAsync(id);
        return entryResult.IsFailed
            ? Result.Fail<GetItemEntryDto>(entryResult.Errors)
            : Result.Ok(MapToDto(entryResult.Value));
    }

    public async Task<Result<Guid>> CreateAsync(CreateItemEntryDto dto)
    {
        var entryResult = ItemEntry.Create(
            ownerId: dto.OwnerId,
            itemTypeId: dto.ItemTypeId,
            pseudonym: dto.Pseudonym);

        return entryResult.IsFailed
            ? Result.Fail<Guid>(entryResult.Errors)
            : await _itemEntriesRepository.CreateAsync(entryResult.Value);
    }

    private static GetItemEntryDto MapToDto(ItemEntry entry) =>
        new(
            Id: entry.Id,
            OwnerId: entry.OwnerId,
            ItemTypeId: entry.ItemTypeId,
            Pseudonym: entry.Pseudonym,
            CreatedAt: entry.CreatedAt);
}
