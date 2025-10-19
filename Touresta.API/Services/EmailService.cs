using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
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

            Console.WriteLine($"🎯 STARTING EMAIL SENDING PROCESS...");
            Console.WriteLine($"   From: {senderEmail} -> To: {toEmail}");
            Console.WriteLine($"   OTP: {otpCode}");

            using (var client = new SmtpClient(smtpServer, port))
            {
                // إعدادات شاملة
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 60000; // 60 second

                // إعدادات الـ TLS
                System.Net.ServicePointManager.SecurityProtocol =
                    System.Net.SecurityProtocolType.Tls12 |
                    System.Net.SecurityProtocolType.Tls11 |
                    System.Net.SecurityProtocolType.Tls;

                client.SendCompleted += (s, e) => {
                    if (e.Error != null)
                        Console.WriteLine($"❌ SEND COMPLETED WITH ERROR: {e.Error.Message}");
                    else
                        Console.WriteLine($"✅ SEND COMPLETED SUCCESSFULLY");
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = $"Touresta Verification Code: {otpCode}",
                    Body = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #2563eb;'>Touresta Admin Verification</h2>
    <p>Your verification code is:</p>
    <h1 style='font-size: 36px; color: #7c3aed;'>{otpCode}</h1>
    <p><strong>Expires in 10 minutes</strong></p>
    <p style='color: #666;'>If you didn't request this, please ignore this email.</p>
</body>
</html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                // إضافة headers علشان نتجنب الـ spam
                mailMessage.Headers.Add("X-Priority", "1");
                mailMessage.Headers.Add("X-Mailer", "TourestaAPI");

                Console.WriteLine($"📧 ATTEMPTING TO SEND...");

                await client.SendMailAsync(mailMessage);

                Console.WriteLine($"✅ EMAIL SENT TO SMTP SERVER: {toEmail}");
                return true;
            }
        }
        catch (SmtpFailedRecipientException ex)
        {
            Console.WriteLine($"❌ RECIPIENT FAILED: {ex.Message}");
            Console.WriteLine($"   Failed: {ex.FailedRecipient}");
            return false;
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"❌ SMTP ERROR: {ex.Message}");
            Console.WriteLine($"   Status: {ex.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GENERAL ERROR: {ex.Message}");
            return false;
        }
    }
}