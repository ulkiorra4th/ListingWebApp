using FluentResults;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;

namespace ListingWebApp.Application.Abstractions;

public interface ICurrenciesService
{
    Task<Result<GetCurrencyDto>> GetByCodeAsync(string currencyCode);
    Task<Result<List<GetCurrencyDto>>> GetAllAsync();
    Task<Result> AddAsync(CreateCurrencyDto dto);
    Task<Result> UpdateIconAsync(
        string currencyCode,
        Stream? content,
        string fileExtension,
        string contentType,
        CancellationToken ct = default);
}
