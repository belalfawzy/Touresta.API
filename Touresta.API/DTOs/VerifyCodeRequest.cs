namespace Touresta.API.DTOs
{
    public class VerifyCodeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}