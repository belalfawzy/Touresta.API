using System.ComponentModel.DataAnnotations;

namespace Touresta.API.DTOs.Auth
{
    /// <summary>
    /// Request to register a new user account.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Display name for the user.</summary>
        /// <example>JohnDoe</example>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>Account password.</summary>
        /// <example>SecurePass@123</example>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>Phone number (optional).</summary>
        /// <example>+201234567890</example>
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        /// <summary>Gender (optional).</summary>
        /// <example>Male</example>
        public string? Gender { get; set; }

        /// <summary>Date of birth (optional).</summary>
        /// <example>1995-06-15</example>
        public DateTime? BirthDate { get; set; }

        /// <summary>Country of residence (optional).</summary>
        /// <example>Egypt</example>
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string? Country { get; set; }

        /// <summary>Profile image URL (optional, defaults to a placeholder).</summary>
        /// <example>/images/users/default.png</example>
        public string? ProfileImageUrl { get; set; }
    }
}
