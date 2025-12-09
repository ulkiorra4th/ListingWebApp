using System.Net.Mail;
using ListingWebApp.Infrastructure.Email.Models;

namespace ListingWebApp.Infrastructure.Email.Abstractions;

public interface IEmailSender
{
    public void ConfigureSender(string senderAddress, string displayName, string senderMailPassword, string host,
        int port);
    
    public void SetSenderAddress(string senderAddress, string displayName, string senderMailPassword);
    
    public void SetSenderAddress(MailAddress sender, string senderMailPassword);
    
    public void ConfigureSmtp(string host, int port);
    
    public Task SendMailAsync(MailAddress receiver, EmailContent content);
    
    public Task SendMailAsync(string receiverMailAddress, EmailContent content);
}