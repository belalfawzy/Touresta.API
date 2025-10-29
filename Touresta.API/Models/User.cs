namespace Touresta.API.Models
{
    public class User
    {
        public int Id { get; set; }
        //خلتهم فيها ايمبتي علشان اشيل الورنينج لانه معصبني 
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? GoogleId { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? VerificationCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImageUrl { get; set; }

        public DateTime? VerificationCodeExpiry { get; set; }
    }
}
