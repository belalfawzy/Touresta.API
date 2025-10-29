namespace Touresta.API.DTOs
{
    public class GoogleLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

}
