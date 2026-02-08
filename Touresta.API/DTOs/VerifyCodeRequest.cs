namespace Touresta.API.DTOs
{
    /// <summary>
    /// Request to verify a 6-digit code sent via email.
    /// </summary>
    public class VerifyCodeRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>6-digit verification code.</summary>
        /// <example>123456</example>
        public string Code { get; set; } = string.Empty;
    }
}
