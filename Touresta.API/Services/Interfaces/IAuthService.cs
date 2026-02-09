using Touresta.API.DTOs.Auth;
using Touresta.API.Models;

namespace Touresta.API.Services.Interfaces
{
    public interface IAuthService
    {
        // User methods
        (bool Success, string Message) CheckEmail(string email);
        (bool Success, string Token, string Message) VerifyPassword(string email, string password);
        Task<(bool Success, string Message, string? UserId)> RegisterAsync(RegisterRequest req);
        (bool Success, string Token, string Message) VerifyGoogleCode(string email, string code);
        Task<(bool Success, string Token, string Message)> RegisterWithGoogleAsync(GoogleRegisterRequest req);
        (bool Success, string Message, string Code) GoogleLogin(string email);
        (bool Success, string Token, string Message) VerifyCode(string email, string code);
        Task<(bool Success, string Email, string Message)> VerifyGoogleAccountAsync(string idToken);

        // Password reset
        Task<(bool Success, string Message)> SendForgotPasswordCodeAsync(string email);
        Task<(bool Success, string Message)> ResetPasswordAsync(string email, string code, string newPassword);
        Task<(bool Success, string Message)> ResendVerificationCodeAsync(string email);

        // Admin methods
        (bool Success, string Message, Admin? Admin) CheckAdminEmail(string email);
        Task<(bool Success, string Message, string? Otp)> AdminPasswordLoginWithOtpAsync(string email, string password);
        Task<(bool Success, string Message, string? Otp)> GoogleAdminLogin(string email);
        (bool Success, string Token, string Message) VerifyAdminOtp(string email, string otp);
    }
}
