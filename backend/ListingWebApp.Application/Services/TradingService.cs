using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Services;

internal sealed class TradingService : ITradingService
{
    private readonly IListingsRepository _listingsRepository;
    private readonly IItemEntriesRepository _itemEntriesRepository;
    private readonly IWalletsRepository _walletsRepository;
    private readonly ITradeTransactionsRepository _tradeTransactionsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TradingService(
        IListingsRepository listingsRepository,
        IItemEntriesRepository itemEntriesRepository,
        IWalletsRepository walletsRepository,
        ITradeTransactionsRepository tradeTransactionsRepository,
        IUnitOfWork unitOfWork)
    {
        _listingsRepository = listingsRepository;
        _itemEntriesRepository = itemEntriesRepository;
        _walletsRepository = walletsRepository;
        _tradeTransactionsRepository = tradeTransactionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetTradeTransactionDto>> PurchaseAsync(PurchaseRequestDto dto)
    {
        var now = DateTime.UtcNow;

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var listingResult = await _listingsRepository.GetByIdAsync(dto.ListingId);
            if (listingResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(listingResult.Errors);

            var listing = listingResult.Value;

            if (listing.Status is not ListingStatus.Approved)
                return Result.Fail<GetTradeTransactionDto>(new ValidationError(nameof(Listing), "Listing is not available for purchase."));

            if (listing.SellerId == dto.BuyerAccountId)
                return Result.Fail<GetTradeTransactionDto>(new ValidationError(nameof(Listing), "Seller cannot buy own listing."));

            // Debit buyer
            var debitResult = await _walletsRepository.DecreaseBalanceAsync(
                dto.BuyerAccountId,
                listing.CurrencyCode,
                listing.PriceAmount,
                now);

            if (debitResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(debitResult.Errors);

            // Ensure seller wallet exists
            var sellerWalletResult = await _walletsRepository.GetByIdAsync(listing.SellerId, listing.CurrencyCode);
            if (sellerWalletResult.IsFailed)
            {
                var sellerWalletCreate = Wallet.Create(
                    currencyCode: listing.CurrencyCode,
                    accountId: listing.SellerId,
                    balance: 0,
                    lastTransactionDate: null,
                    isActive: true);

                if (sellerWalletCreate.IsFailed)
                    return Result.Fail<GetTradeTransactionDto>(sellerWalletCreate.Errors);

                var upsertResult = await _walletsRepository.UpsertAsync(sellerWalletCreate.Value);
                if (upsertResult.IsFailed)
                    return Result.Fail<GetTradeTransactionDto>(upsertResult.Errors);
            }

            var creditResult = await _walletsRepository.IncreaseBalanceAsync(
                listing.SellerId,
                listing.CurrencyCode,
                listing.PriceAmount,
                now);

            if (creditResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(creditResult.Errors);

            // Transfer ownership of the item entry
            var transferResult = await _itemEntriesRepository.TransferOwnershipAsync(listing.ItemEntryId, dto.BuyerAccountId);
            if (transferResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(transferResult.Errors);

            // Close listing
            var closeResult = await _listingsRepository.UpdateStatusAsync(listing.Id, ListingStatus.Closed);
            if (closeResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(closeResult.Errors);

            var transactionResult = TradeTransaction.Create(
                buyerAccountId: dto.BuyerAccountId,
                sellerAccountId: listing.SellerId,
                listingId: listing.Id,
                currencyCode: listing.CurrencyCode,
                amount: listing.PriceAmount,
                isSuspicious: dto.IsSuspicious,
                transactionDate: now);

            if (transactionResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(transactionResult.Errors);

            var createResult = await _tradeTransactionsRepository.CreateAsync(transactionResult.Value);
            if (createResult.IsFailed)
                return Result.Fail<GetTradeTransactionDto>(createResult.Errors);

            return Result.Ok(MapToDto(transactionResult.Value));
        });
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
