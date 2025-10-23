namespace Touresta.API.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
