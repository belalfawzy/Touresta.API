namespace Touresta.API.DTOs
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;            
        public string NewPassword { get; set; } = string.Empty;

    }
}
