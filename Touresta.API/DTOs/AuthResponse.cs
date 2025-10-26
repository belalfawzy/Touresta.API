namespace Touresta.API.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public UserData? User { get; set; }
        public AdminData? Admin { get; set; }
    }
}
