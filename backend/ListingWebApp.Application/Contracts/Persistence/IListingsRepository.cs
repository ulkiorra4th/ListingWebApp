using FluentResults;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IListingsRepository
{
    Task<Result<Listing>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(Listing listing);
    Task<Result> UpdateStatusAsync(Guid id, ListingStatus status);
}
