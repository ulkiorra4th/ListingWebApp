using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Services;

internal sealed class ItemsService : IItemsService
{
    private readonly IItemsRepository _itemsRepository;

    public ItemsService(IItemsRepository itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    public async Task<Result<GetItemDto>> GetByIdAsync(Guid id)
    {
        var itemResult = await _itemsRepository.GetByIdAsync(id);
        return itemResult.IsFailed
            ? Result.Fail<GetItemDto>(itemResult.Errors)
            : Result.Ok(MapToDto(itemResult.Value));
    }

    public async Task<Result<Guid>> CreateAsync(CreateItemDto dto)
    {
        var itemResult = Item.Create(
            name: dto.Name,
            rarity: dto.Rarity,
            basePrice: dto.BasePrice,
            description: dto.Description,
            iconKey: dto.IconKey,
            releaseDate: dto.ReleaseDate,
            isTrading: dto.IsTrading);

        return itemResult.IsFailed
            ? Result.Fail<Guid>(itemResult.Errors)
            : await _itemsRepository.CreateAsync(itemResult.Value);
    }

    public async Task<Result> UpdateIconKeyAsync(UpdateItemIconDto dto)
        => await _itemsRepository.UpdateIconKeyAsync(dto.ItemId, dto.IconKey);

    private static GetItemDto MapToDto(Item item) =>
        new(
            Id: item.Id,
            Name: item.Name,
            Rarity: item.Rarity,
            BasePrice: item.BasePrice,
            Description: item.Description,
            IconKey: item.IconKey,
            ReleaseDate: item.ReleaseDate,
            IsTrading: item.IsTrading);
}
