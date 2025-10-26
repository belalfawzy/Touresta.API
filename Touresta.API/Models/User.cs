namespace Touresta.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }  
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? GoogleId { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? VerificationCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImageUrl { get; set; }
    }
}
