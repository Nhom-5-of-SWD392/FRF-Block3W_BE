using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
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
    private readonly ISmtpClient _smtpClient;
    private readonly IConfiguration _configuration;
    private readonly MailSetupModel _mailSetup;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public EmailService(
            IConfiguration configuration, 
            IOptions<MailSetupModel> mailSetup, 
            DataContext dataContext, 
            IMapper mapper, 
            IServiceProvider serviceProvider, 
            ISmtpClient smtpClient)
    {
        _configuration = configuration;
        _mailSetup = mailSetup.Value;
        _dataContext = dataContext;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
        _smtpClient = smtpClient;
    }

    public async Task SendResetPasswordEmailAsync(User user, string email, string resetLink)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email)) throw new AppException(ErrorMessage.RecipientNotExist);

            if (string.IsNullOrWhiteSpace(resetLink)) throw new AppException("Reset link cannot be null or empty.");

            var message = new MimeMessage();

            message.Sender = MailboxAddress.Parse(_mailSetup.FromEmail);

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Đặt lại mật khẩu";

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
                            Đặt lại mật khẩu của bạn
                        </h1>

                        <!-- Nội dung chính -->
                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                            Xin chào {user.FirstName + " " + user.LastName},
                        </p>
                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                            Bạn đã yêu cầu đặt lại mật khẩu. Để tiếp tục, vui lòng 
                            <a href='{resetLink}' style='color:#152C88; text-decoration:none;'>nhấn vào đây</a> 
                            để tạo mật khẩu mới cho tài khoản của bạn.
                        </p>
                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi nếu bạn có bất kỳ lo ngại nào về bảo mật tài khoản.
                        </p>
                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                            Liên kết này sẽ hết hạn sau 24 giờ để đảm bảo an toàn cho bạn.
                        </p>

                        <!-- Phần ngăn cách (divider) -->
                        <hr style='border:none; border-top:1px solid #e0e0e0; margin:20px 0;' />

                        <!-- Chữ ký và thông tin liên hệ -->
                        <p style='font-size:16px; line-height:1.6; margin-bottom:20px;'>
                            Trân trọng,<br>
                            Đội ngũ hỗ trợ Food Hub
                        </p>
                        <p style='font-size:14px; line-height:1.6; margin-bottom:20px;'>
                            Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ 
                            <a href='mailto:support.team@foodhub.com' style='color:#152C88; text-decoration:none;'>
                                support.team@foodhub.com
                            </a>.
                        </p>

                        <!-- Footer -->
                        <div style='text-align:center; font-size:12px; color:#999999;'>
                            Đây là email tự động. Vui lòng không trả lời email này.
                        </div>
                    </div>
"

            };

            message.Body = builder.ToMessageBody();

            await _smtpClient.SendEmailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw new AppException("Failed to send email. Please try again.");
        }
    }
}