using ListingWebApp.Application.Dto.Messages;

namespace ListingWebApp.Application.Abstractions;

public interface IAccountVerificationQueue
{
    ValueTask QueueAsync(AccountVerificationMessageDto message, CancellationToken cancellationToken = default);
    IAsyncEnumerable<AccountVerificationMessageDto> ReadAllAsync(CancellationToken cancellationToken);
}