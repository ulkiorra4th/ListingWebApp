using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Abstractions;

public interface IListingsService
{
    Task<Result<GetListingDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateListingDto dto);
    Task<Result> UpdateStatusAsync(Guid id, ListingStatus status);
}
