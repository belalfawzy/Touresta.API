using System.ComponentModel.DataAnnotations;

namespace RAFIQ.API.DTOs.Auth
{
    /// <summary>
    /// Request to reset a user's password using a verification code.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>6-digit verification code received via email.</summary>
        /// <example>123456</example>
        [Required(ErrorMessage = "Verification code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits.")]
        public string Code { get; set; } = string.Empty;

        /// <summary>The new password to set.</summary>
        /// <example>NewSecurePass@456</example>
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
