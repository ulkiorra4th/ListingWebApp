using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Services;

internal sealed class ListingsService : IListingsService
{
    private readonly IListingsRepository _listingsRepository;

    public ListingsService(IListingsRepository listingsRepository)
    {
        _listingsRepository = listingsRepository;
    }

    public async Task<Result<GetListingDto>> GetByIdAsync(Guid id)
    {
        var listingResult = await _listingsRepository.GetByIdAsync(id);
        return listingResult.IsFailed
            ? Result.Fail<GetListingDto>(listingResult.Errors)
            : Result.Ok(MapToDto(listingResult.Value));
    }

    public async Task<Result<Guid>> CreateAsync(CreateListingDto dto)
    {
        var listingResult = Listing.Create(
            sellerId: dto.SellerId,
            itemEntryId: dto.ItemEntryId,
            currencyCode: dto.CurrencyCode,
            priceAmount: dto.PriceAmount,
            status: dto.Status);

        return listingResult.IsFailed
            ? Result.Fail<Guid>(listingResult.Errors)
            : await _listingsRepository.CreateAsync(listingResult.Value);
    }

    public async Task<Result> UpdateStatusAsync(Guid id, ListingStatus status)
        => await _listingsRepository.UpdateStatusAsync(id, status);

    private static GetListingDto MapToDto(Listing listing) =>
        new(
            Id: listing.Id,
            SellerId: listing.SellerId,
            ItemEntryId: listing.ItemEntryId,
            CurrencyCode: listing.CurrencyCode,
            PriceAmount: listing.PriceAmount,
            Status: listing.Status,
            CreatedAt: listing.CreatedAt,
            UpdatedAt: listing.UpdatedAt);
}
