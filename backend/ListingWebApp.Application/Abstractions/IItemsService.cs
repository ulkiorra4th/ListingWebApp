using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface IItemsService
{
    Task<Result<GetItemDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateItemDto dto);
    Task<Result> UpdateIconAsync(
        Guid itemId,
        Stream content,
        string fileExtension,
        string contentType,
        CancellationToken ct = default);
}
