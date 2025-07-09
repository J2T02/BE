using System.Net.Mail;
using MimeKit;
using System.Threading.Tasks;

namespace SWP.Service.Email
{
    public class EmailService
    {
        private readonly string _emailFrom;
        private readonly string _emailPassword;

        public EmailService(IConfiguration config)
        {
            _emailFrom = config["EmailSettings:From"];
            _emailPassword = config["EmailSettings:AppPassword"];
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailFrom));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Your OTP Code";
            email.Body = new TextPart("html")
            {
                Text = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 8px; max-width: 500px; margin: auto;'>
            <h2 style='color: #1890ff;'>🔐 Xác thực OTP</h2>
            <p>Xin chào,</p>
            <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn.</p>
            <p>Mã OTP của bạn là:</p>
            <div style='font-size: 24px; font-weight: bold; color: #d32f2f; margin: 16px 0;'>{otp}</div>
            <p>Mã OTP này chỉ có hiệu lực trong vòng 5 phút.</p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
            <hr />
            <p style='font-size: 12px; color: #888;'>Trân trọng,<br/>Fertility Journey Team</p>
        </div>"
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailFrom, _emailPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
