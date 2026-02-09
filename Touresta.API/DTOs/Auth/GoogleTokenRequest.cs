namespace Touresta.API.DTOs.Auth
{
    /// <summary>
    /// Request to verify a Google ID token.
    /// </summary>
    public class GoogleTokenRequest
    {
        /// <summary>Google ID token from client-side authentication.</summary>
        /// <example>eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string IdToken { get; set; } = string.Empty;
    }
}
