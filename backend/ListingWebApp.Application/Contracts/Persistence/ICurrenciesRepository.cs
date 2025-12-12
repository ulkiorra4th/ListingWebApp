using FluentResults;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface ICurrenciesRepository
{
    Task<Result<Currency>> GetByCodeAsync(string currencyCode);
    Task<Result<List<Currency>>> GetAllAsync();
    Task<Result> AddAsync(Currency currency);
    Task<Result> UpdateIconKeyAsync(string currencyCode, string? iconKey);
}
