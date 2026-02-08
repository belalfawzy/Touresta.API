namespace Touresta.API.DTOs
{
    /// <summary>
    /// Login request with email and password.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>User's password.</summary>
        /// <example>SecurePass@123</example>
        public string Password { get; set; } = string.Empty;
    }
}
