using System.ComponentModel.DataAnnotations;

namespace RAFIQ.API.DTOs.Auth
{
    /// <summary>
    /// Request to register or activate an account using Google credentials.
    /// </summary>
    public class GoogleRegisterRequest
    {
        /// <summary>User's email from Google account.</summary>
        /// <example>john@gmail.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Google unique user ID.</summary>
        /// <example>google-uid-123456</example>
        [Required(ErrorMessage = "Google ID is required.")]
        public string GoogleId { get; set; } = string.Empty;

        /// <summary>User's display name from Google.</summary>
        /// <example>John Doe</example>
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Profile image URL from Google (optional).</summary>
        /// <example>https://lh3.googleusercontent.com/a/photo.jpg</example>
        public string? ProfileImageUrl { get; set; }
    }
}
