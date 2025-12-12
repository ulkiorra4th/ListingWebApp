using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IItemsRepository
{
    Task<Result<Item>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(Item item);
    Task<Result> UpdateIconKeyAsync(Guid id, string? iconKey);
}
