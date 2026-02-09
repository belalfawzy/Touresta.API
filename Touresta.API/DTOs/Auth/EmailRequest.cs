namespace Touresta.API.DTOs.Auth
{
    /// <summary>
    /// Request containing only an email address.
    /// </summary>
    public class EmailRequest
    {
        /// <summary>User's email address.</summary>
        /// <example>john@example.com</example>
        public string Email { get; set; } = string.Empty;
    }
}
