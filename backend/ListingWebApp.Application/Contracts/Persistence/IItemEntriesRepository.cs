using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IItemEntriesRepository
{
    Task<Result<ItemEntry>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(ItemEntry entry);
}
