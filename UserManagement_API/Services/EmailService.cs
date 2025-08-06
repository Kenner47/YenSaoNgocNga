using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task SendVerifyEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail))
                    throw new ArgumentException("Email không hợp lệ.");

                if (string.IsNullOrEmpty(subject))
                    throw new ArgumentException("Subject không hợp lệ.");

                if (string.IsNullOrEmpty(body))
                    throw new ArgumentException("Body không hợp lệ.");

                var smtpSettings = _config.GetSection("SmtpSettings");
                if (smtpSettings == null)
                    throw new Exception("Cấu hình SMTP không tìm thấy.");

                string fromEmail = smtpSettings["Username"];
                string password = smtpSettings["Password"];
                string smtpHost = smtpSettings["Host"];
                int smtpPort = int.Parse(smtpSettings["Port"]);
                bool enableSsl = bool.Parse(smtpSettings["EnableSsl"]);

                // Log debug
                Console.WriteLine($"=== EMAIL SENDING DEBUG ===");
                Console.WriteLine($"SMTP Host: {smtpHost}:{smtpPort}");
                Console.WriteLine($"From Email: {fromEmail}");
                Console.WriteLine($"To Email: {toEmail}");
                Console.WriteLine($"SSL Enabled: {enableSsl}");
                Console.WriteLine($"Password Length: {password?.Length ?? 0}");
                Console.WriteLine("==========================");

                if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(smtpHost))
                    throw new Exception("Cấu hình SMTP không hợp lệ.");

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Yến Sào Ngọc Nga", fromEmail));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;

                //
                var htmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f4f4;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                            <h2 style='color: #333; text-align: center; border-bottom: 2px solid #007bff; padding-bottom: 10px;'>
                                🔐 Xác Thực Tài Khoản
                            </h2>
                            <p style='font-size: 16px; color: #666;'>Chào bạn!</p>
                            <p style='font-size: 16px; color: #666;'>
                                Cảm ơn bạn đã đăng ký tài khoản tại <strong>Yến Sào Ngọc Nga</strong>.
                            </p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <p style='font-size: 18px; color: #333; margin-bottom: 10px;'>Mã OTP của bạn là:</p>
                                <div style='display: inline-block; background-color: #007bff; color: white; padding: 15px 25px; border-radius: 8px; font-size: 24px; font-weight: bold; letter-spacing: 3px;'>
                                    {body.Replace("Mã OTP của bạn là: <b>", "").Replace("</b>. Vui lòng nhập mã này để kích hoạt tài khoản.", "")}
                                </div>
                            </div>
                            <p style='font-size: 14px; color: #999; text-align: center;'>
                                ⏰ Mã này sẽ hết hạn sau <strong>5 phút</strong>
                            </p>
                            <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                            <p style='font-size: 12px; color: #ccc; text-align: center;'>
                                © 2025 Yến Sào Ngọc Nga. All rights reserved.
                            </p>
                        </div>
                    </body>
                    </html>";

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                email.Body = bodyBuilder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    Console.WriteLine("Connecting to SMTP server...");
                    await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

                    Console.WriteLine("Authenticating...");
                    await smtp.AuthenticateAsync(fromEmail, password);

                    Console.WriteLine("Sending email...");
                    await smtp.SendAsync(email);

                    Console.WriteLine("Disconnecting...");
                    await smtp.DisconnectAsync(true);

                    Console.WriteLine("✅ Email sent successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}