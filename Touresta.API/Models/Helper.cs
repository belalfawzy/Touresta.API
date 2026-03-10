using RAFIQ.API.Enums;

namespace RAFIQ.API.Models
{
    public class Helper
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string HelperId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public string FullName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? NationalIdPhoto { get; set; }
        public string? CriminalRecordFile { get; set; }


        public bool HasCar { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

        public bool IsBanned { get; set; } = false;
        public string? BanReason { get; set; }
        public DateTime? BannedAt { get; set; }
        public string? BannedByAdminId { get; set; }

        public bool IsSuspended { get; set; } = false;
        public string? SuspensionReason { get; set; }
        public DateTime? SuspendedAt { get; set; }
        public string? SuspendedByAdminId { get; set; }

        public string? RejectionReason { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByAdminId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Car? Car { get; set; }
        public List<HelperLanguage> Languages { get; set; } = new();
        public List<Certificate> Certificates { get; set; } = new();
        public List<DrugTest> DrugTests { get; set; } = new();
    }
}
