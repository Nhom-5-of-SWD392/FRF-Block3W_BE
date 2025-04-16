using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.Utilities;

namespace Service.Core;

public interface IEmailService
{
    Task SendResetPasswordEmailAsync(User user, string email, string resetLink);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly MailSetupModel _mailSetup;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public EmailService(IConfiguration configuration, IOptions<MailSetupModel> mailSetup, DataContext dataContext, IMapper mapper, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _mailSetup = mailSetup.Value;
        _dataContext = dataContext;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    public async Task SendResetPasswordEmailAsync(User user, string email, string resetLink)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email)) throw new AppException("Recipient email cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(resetLink)) throw new AppException("Reset link cannot be null or empty.");

            var message = new MimeMessage();

            message.Sender = MailboxAddress.Parse(_mailSetup.FromEmail);

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Reset Your Password";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                                    <!-- Container tổng, tối giản và tập trung vào nội dung -->
                                    <div style='max-width:600px; margin:40px auto; padding:20px; font-family:Arial, sans-serif; color:#333333;'>
    
                                        <!-- Logo -->
                                        <div style='text-align:center; margin-bottom:30px;'>
                                            <img src='https://play-lh.googleusercontent.com/enSxIOa7dsax8HUiP094JFzSmKeoHpnbWk8rRGYXaSFm30GvUaLXmnBKyFC3zvavs-8'
                                                 alt='Company Logo'
                                                 style='max-width:180px; height:auto;' />
                                        </div>
    
                                        <!-- Tiêu đề -->
                                        <h1 style='font-size:24px; margin:0 0 20px; text-align:center;'>
                                            Reset Your Password
                                        </h1>
    
                                        <!-- Nội dung chính -->
                                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                                            Dear {user.FirstName + " " + user.LastName},
                                        </p>
                                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                                            You have requested to reset your password. To proceed, please 
                                            <a href='{resetLink}' style='color:#152C88; text-decoration:none;'>click here</a> 
                                            to set a new password for your account.
                                        </p>
                                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                                            If you did not request a password reset, please ignore this email or contact us if you have concerns about your account's security.
                                        </p>
                                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                                            This link will expire in 24 hours for your security.
                                        </p>

                                        <!-- Phần ngăn cách (divider) -->
                                        <hr style='border:none; border-top:1px solid #e0e0e0; margin:20px 0;' />

                                        <!-- Chữ ký và thông tin liên hệ -->
                                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                                            Thank you,<br>
                                            The Food Hub Support Team
                                        </p>
                                        <p style='font-size:14px; line-height:1.6; margin-bottom:20px;'>
                                            If you have any questions, please feel free to contact us at 
                                            <a href='mailto:support.team@foodhub.com' style='color:#152C88; text-decoration:none;'>
                                                support.team@foodhub.com
                                            </a>.
                                        </p>

                                        <!-- Footer -->
                                        <div style='text-align:center; font-size:12px; color:#999999;'>
                                            This is an automated email. Please do not reply to this email.
                                        </div>
                                    </div>

                                "
            };

            message.Body = builder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_mailSetup.SmtpServer, _mailSetup.SmtpPort, SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(_mailSetup.FromEmail, _mailSetup.Password);

            await smtpClient.SendAsync(message);

            await smtpClient.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw new AppException("Failed to send email. Please try again.");
        }
    }
}