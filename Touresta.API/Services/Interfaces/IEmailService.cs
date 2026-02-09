namespace Touresta.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmail(string toEmail, string otpCode);
    }
}
