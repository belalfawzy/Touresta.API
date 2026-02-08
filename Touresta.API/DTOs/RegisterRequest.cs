namespace Touresta.API.DTOs
{
    /// <summary>
    /// Request to register a new user account.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>Display name for the user.</summary>
        /// <example>JohnDoe</example>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Account password.</summary>
        /// <example>SecurePass@123</example>
        public string Password { get; set; } = string.Empty;

        /// <summary>Phone number (optional).</summary>
        /// <example>+201234567890</example>
        public string? PhoneNumber { get; set; }

        /// <summary>Gender (optional).</summary>
        /// <example>Male</example>
        public string? Gender { get; set; }

        /// <summary>Date of birth (optional).</summary>
        /// <example>1995-06-15</example>
        public DateTime? BirthDate { get; set; }

        /// <summary>Country of residence (optional).</summary>
        /// <example>Egypt</example>
        public string? Country { get; set; }

        /// <summary>Profile image URL (optional, defaults to a placeholder).</summary>
        /// <example>/images/users/default.png</example>
        public string? ProfileImageUrl { get; set; }
    }
}
