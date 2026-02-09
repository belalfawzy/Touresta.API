using System.Net;
using System.Net.Mail;
using Touresta.API.Services.Interfaces;

namespace Touresta.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendOtpEmail(string toEmail, string otpCode)
        {
            try
            {
                var smtpServer = _config["EmailSettings:SmtpServer"];
                var port = int.Parse(_config["EmailSettings:Port"]);
                var senderEmail = _config["EmailSettings:SenderEmail"];
                var senderName = _config["EmailSettings:SenderName"];
                var password = _config["EmailSettings:Password"];

                using (var client = new SmtpClient(smtpServer, port))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(senderEmail, password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 60000;

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, senderName),
                        Subject = $"Touresta Verification Code: {otpCode}",
                        Body = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #2563eb;'>Touresta Verification</h2>
    <p>Your verification code is:</p>
    <h1 style='font-size: 36px; color: #7c3aed;'>{otpCode}</h1>
    <p><strong>Expires in 10 minutes</strong></p>
    <p style='color: #666;'>If you didn't request this, please ignore this email.</p>
</body>
</html>",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);
                    mailMessage.Headers.Add("X-Priority", "1");
                    mailMessage.Headers.Add("X-Mailer", "TourestaAPI");

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}
