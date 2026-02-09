using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class Helper
    {
        public int Id { get; set; }
        public string HelperId { get; set; } = string.Empty;

        // FK to Users table (1:1)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Profile
        public string FullName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Documents
        public string? NationalIdPhoto { get; set; }
        public string? CriminalRecordFile { get; set; }

        // Flags
        public bool HasCar { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

        // Admin review
        public string? RejectionReason { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int? ReviewedByAdminId { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Car? Car { get; set; }
        public List<HelperLanguage> Languages { get; set; } = new();
        public List<Certificate> Certificates { get; set; } = new();
        public List<DrugTest> DrugTests { get; set; } = new();
    }
}
