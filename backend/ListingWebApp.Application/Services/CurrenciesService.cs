using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Services;

internal sealed class CurrenciesService : ICurrenciesService
{
    private readonly ICurrenciesRepository _currenciesRepository;

    public CurrenciesService(ICurrenciesRepository currenciesRepository)
    {
        _currenciesRepository = currenciesRepository;
    }

    public async Task<Result<GetCurrencyDto>> GetByCodeAsync(string currencyCode)
    {
        var currencyResult = await _currenciesRepository.GetByCodeAsync(currencyCode);
        return currencyResult.IsFailed
            ? Result.Fail<GetCurrencyDto>(currencyResult.Errors)
            : Result.Ok(MapToDto(currencyResult.Value));
    }

    public async Task<Result<List<GetCurrencyDto>>> GetAllAsync()
    {
        var currenciesResult = await _currenciesRepository.GetAllAsync();
        if (currenciesResult.IsFailed)
            return Result.Fail<List<GetCurrencyDto>>(currenciesResult.Errors);

        var dtos = currenciesResult.Value.Select(MapToDto).ToList();
        return Result.Ok(dtos);
    }

    public async Task<Result> AddAsync(CreateCurrencyDto dto)
    {
        var currencyResult = Currency.Create(
            currencyCode: dto.CurrencyCode,
            name: dto.Name,
            description: dto.Description,
            iconKey: dto.IconKey,
            minTransferAmount: dto.MinTransferAmount,
            maxTransferAmount: dto.MaxTransferAmount,
            isTransferAllowed: dto.IsTransferAllowed);

        return currencyResult.IsFailed
            ? Result.Fail(currencyResult.Errors)
            : await _currenciesRepository.AddAsync(currencyResult.Value);
    }

    public async Task<Result> UpdateIconKeyAsync(UpdateCurrencyIconDto dto)
        => await _currenciesRepository.UpdateIconKeyAsync(dto.CurrencyCode, dto.IconKey);

    private static GetCurrencyDto MapToDto(Currency currency) =>
        new(
            CurrencyCode: currency.CurrencyCode,
            Name: currency.Name,
            Description: currency.Description,
            IconKey: currency.IconKey,
            MinTransferAmount: currency.MinTransferAmount,
            MaxTransferAmount: currency.MaxTransferAmount,
            IsTransferAllowed: currency.IsTransferAllowed);
}
