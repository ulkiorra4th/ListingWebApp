using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Services;

internal sealed class ItemsService : IItemsService
{
    private readonly IItemsRepository _itemsRepository;
    private readonly IObjectStorageService _objectStorageService;

    public ItemsService(IItemsRepository itemsRepository, IObjectStorageService objectStorageService)
    {
        _itemsRepository = itemsRepository;
        _objectStorageService = objectStorageService;
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

    public async Task<Result> UpdateIconAsync(
        Guid itemId,
        Stream? content,
        string fileExtension,
        string contentType,
        CancellationToken ct = default)
    {
        if (content is null)
        {
            return Result.Fail("Icon content is required.");
        }

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return Result.Fail("File extension is required.");
        }

        var normalizedExtension = fileExtension.StartsWith(".", StringComparison.Ordinal)
            ? fileExtension
            : $".{fileExtension}";

        var iconKey = $"items/{itemId}{normalizedExtension}";

        var uploadResult = await _objectStorageService.UploadAsync(iconKey, content, contentType, ct);
        if (uploadResult.IsFailed)
        {
            return Result.Fail(uploadResult.Errors);
        }

        return await _itemsRepository.UpdateIconKeyAsync(itemId, iconKey);
    }

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
