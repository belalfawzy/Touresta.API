namespace Touresta.API.DTOs.Auth
{
    /// <summary>
    /// Request to register or activate an account using Google credentials.
    /// </summary>
    public class GoogleRegisterRequest
    {
        /// <summary>User's email from Google account.</summary>
        /// <example>john@gmail.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>Google unique user ID.</summary>
        /// <example>google-uid-123456</example>
        public string GoogleId { get; set; } = string.Empty;

        /// <summary>User's display name from Google.</summary>
        /// <example>John Doe</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>Profile image URL from Google (optional).</summary>
        /// <example>https://lh3.googleusercontent.com/a/photo.jpg</example>
        public string? ProfileImageUrl { get; set; }
    }
}
