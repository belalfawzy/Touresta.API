using System.ComponentModel.DataAnnotations;

namespace RAFIQ.API.DTOs.Auth
{
    /// <summary>
    /// Request to verify a 6-digit code sent via email.
    /// </summary>
    public class VerifyCodeRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>6-digit verification code.</summary>
        /// <example>123456</example>
        [Required(ErrorMessage = "Verification code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits.")]
        public string Code { get; set; } = string.Empty;
    }
}
