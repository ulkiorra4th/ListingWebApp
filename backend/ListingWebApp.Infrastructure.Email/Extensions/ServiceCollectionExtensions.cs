using ListingWebApp.Infrastructure.Email.Abstractions;
using ListingWebApp.Infrastructure.Email.Options;
using ListingWebApp.Infrastructure.Email.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ListingWebApp.Infrastructure.Email.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailServiceOptions>(configuration.GetSection("EmailServiceOptions"));

        services.TryAddSingleton<IEmailParser, EmailParser>();
        services.AddSingleton<IEmailContentBuilder, EmailContentBuilder>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<EmailServiceOptions>>().Value;

            var htmlParser = sp.GetRequiredService<IEmailParser>();
            var parseResult = htmlParser.Parse(config.VerificationTemplateFilePath);
            if (parseResult.IsFailed)
            {
                throw new Exception(string.Join(';', parseResult.Errors));
            }

            return new EmailContentBuilder(parseResult.Value.Subject, parseResult.Value.Body);
        });

        services.AddSingleton<IEmailSender>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<EmailServiceOptions>>().Value;

            return new EmailSender(
                config.SenderAddress,
                config.DisplayName,
                config.SenderMailPassword,
                config.SmtpHost,
                config.SmtpPort
            );
        });

        return services;
    }
}