using LuxoraStore.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace LuxoraStore.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Luxora Store", _configuration["EmailSettings:FromEmail"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Kode OTP Lupa Password";

                email.Body = new TextPart("plain")
                {
                    Text = $"Halo,\n\nKode OTP kamu: {otpCode}\nBerlaku selama 10 menit.\n\nSalam,\nTim Luxora Store"
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_configuration["EmailSettings:SmtpHost"], int.Parse(_configuration["EmailSettings:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MailKit Send Error] {ex.Message}");
                return false;
            }
        }


    }
}
