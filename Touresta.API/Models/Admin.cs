using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? GoogleId { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }
        public Role Role { get; set; } = Role.Admin;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}