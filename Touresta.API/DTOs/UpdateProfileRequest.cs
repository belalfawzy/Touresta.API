namespace Touresta.API.DTOs
{
    public class UpdateProfileRequest
    {
        public string UserId { get; set; } = string.Empty; 

        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
