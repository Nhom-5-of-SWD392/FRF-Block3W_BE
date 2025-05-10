using Data.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Service.Core;

public interface ISmtpClient
{
    Task SendEmailAsync(MimeMessage message);
}

public class SmtpClientWrapper : ISmtpClient
{
    private readonly MailSetupModel _mailSetup;

    public SmtpClientWrapper(IOptions<MailSetupModel> mailSetup)
    {
        _mailSetup = mailSetup.Value;
    }

    public async Task SendEmailAsync(MimeMessage message)
    {
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailSetup.SmtpServer, _mailSetup.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailSetup.FromEmail, _mailSetup.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}

