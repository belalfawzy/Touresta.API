namespace Touresta.API.DTOs
{
    /// <summary>
    /// Request to reset a user's password using a verification code.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>6-digit verification code received via email.</summary>
        /// <example>123456</example>
        public string Code { get; set; } = string.Empty;

        /// <summary>The new password to set.</summary>
        /// <example>NewSecurePass@456</example>
        public string NewPassword { get; set; } = string.Empty;
    }
}
