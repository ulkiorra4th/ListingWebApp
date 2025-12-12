using FluentResults;

namespace ListingWebApp.Application.Contracts.Persistence;

public interface IUnitOfWork
{
    Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action);
    Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action);
}
