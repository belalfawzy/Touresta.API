using System.ComponentModel.DataAnnotations;

namespace RAFIQ.API.DTOs.Auth
{
    /// <summary>
    /// Login request with email and password.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>User's password.</summary>
        /// <example>SecurePass@123</example>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
