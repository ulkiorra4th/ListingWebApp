using System.Runtime.CompilerServices;
using System.Threading.Channels;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Messages;
using Microsoft.Extensions.Logging;

namespace ListingWebApp.Application.Services;

internal sealed class AccountVerificationQueue : IAccountVerificationQueue
{
    private readonly Channel<AccountVerificationMessageDto> _channel;
    private readonly ILogger<AccountVerificationQueue> _logger;

    public AccountVerificationQueue(ILogger<AccountVerificationQueue> logger)
    {
        _logger = logger;
        var options = new BoundedChannelOptions(capacity: 1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };

        _channel = Channel.CreateBounded<AccountVerificationMessageDto>(options);
    }

    public async ValueTask QueueAsync(AccountVerificationMessageDto message, CancellationToken cancellationToken = default)
    {
        while (await _channel.Writer.WaitToWriteAsync(cancellationToken).ConfigureAwait(false))
        {
            if (!_channel.Writer.TryWrite(message)) continue;
            
            _logger.LogInformation("Account verification email queued.");
            return;
        }

        _logger.LogCritical("Unable to queue account verification message: channel is closed.");
    }

    public async IAsyncEnumerable<AccountVerificationMessageDto> ReadAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
        {
            while (_channel.Reader.TryRead(out var message))
            {
                yield return message;
            }
        }
    }
}