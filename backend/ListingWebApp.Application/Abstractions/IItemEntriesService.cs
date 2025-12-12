using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface IItemEntriesService
{
    Task<Result<GetItemEntryDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateItemEntryDto dto);
}
