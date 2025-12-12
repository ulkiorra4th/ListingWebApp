using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;

namespace ListingWebApp.Application.Services;

internal sealed class TradeTransactionsService : ITradeTransactionsService
{
    private readonly ITradeTransactionsRepository _tradeTransactionsRepository;

    public TradeTransactionsService(ITradeTransactionsRepository tradeTransactionsRepository)
    {
        _tradeTransactionsRepository = tradeTransactionsRepository;
    }

    public async Task<Result<GetTradeTransactionDto>> GetByIdAsync(Guid id)
    {
        var transactionResult = await _tradeTransactionsRepository.GetByIdAsync(id);
        return transactionResult.IsFailed
            ? Result.Fail<GetTradeTransactionDto>(transactionResult.Errors)
            : Result.Ok(MapToDto(transactionResult.Value));
    }

    public async Task<Result<Guid>> CreateAsync(CreateTradeTransactionDto dto)
    {
        var transactionResult = TradeTransaction.Create(
            buyerAccountId: dto.BuyerAccountId,
            sellerAccountId: dto.SellerAccountId,
            listingId: dto.ListingId,
            currencyCode: dto.CurrencyCode,
            amount: dto.Amount,
            isSuspicious: dto.IsSuspicious,
            transactionDate: dto.TransactionDate);

        return transactionResult.IsFailed
            ? Result.Fail<Guid>(transactionResult.Errors)
            : await _tradeTransactionsRepository.CreateAsync(transactionResult.Value);
    }

    private static GetTradeTransactionDto MapToDto(TradeTransaction transaction) =>
        new(
            Id: transaction.Id,
            BuyerAccountId: transaction.BuyerAccountId,
            SellerAccountId: transaction.SellerAccountId,
            ListingId: transaction.ListingId,
            CurrencyCode: transaction.CurrencyCode,
            Amount: transaction.Amount,
            IsSuspicious: transaction.IsSuspicious,
            TransactionDate: transaction.TransactionDate);
}
