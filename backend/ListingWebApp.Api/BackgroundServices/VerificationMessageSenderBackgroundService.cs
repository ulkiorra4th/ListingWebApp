using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Infrastructure.Email.Abstractions;

namespace ListingWebApp.Api.BackgroundServices;

public class VerificationMessageSenderBackgroundService : BackgroundService
{
    private readonly IAccountVerificationQueue _accountVerificationQueue;
    private readonly ILogger<VerificationMessageSenderBackgroundService> _logger;
    private readonly IEmailContentBuilder _emailContentBuilder;
    private readonly IEmailSender _emailSender;

    public VerificationMessageSenderBackgroundService(
        IAccountVerificationQueue accountVerificationQueue, 
        ILogger<VerificationMessageSenderBackgroundService> logger, 
        IEmailContentBuilder emailContentBuilder, 
        IEmailSender emailSender)
    {
        _accountVerificationQueue = accountVerificationQueue;
        _logger = logger;
        _emailContentBuilder = emailContentBuilder;
        _emailSender = emailSender;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting verification message sender.");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Verification message sender started.");

        try
        {
            await foreach (var message in _accountVerificationQueue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var content = _emailContentBuilder.BuildMailContent(message.VerificationCode);
                    await _emailSender.SendMailAsync(message.Email, content);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex, "Error while sending verification message for user with email {Email}",
                        message.Email);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Verification message sender is stopping due to cancellation.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verification message sender terminated unexpectedly.");
        }

        _logger.LogInformation("AccountVerificationWorker stopped.");
    }
}