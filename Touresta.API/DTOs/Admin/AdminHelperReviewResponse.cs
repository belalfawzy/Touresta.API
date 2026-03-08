using Touresta.API.DTOs.Car;
using Touresta.API.DTOs.Certificates;
using Touresta.API.DTOs.DrugTest;
using Touresta.API.DTOs.Languages;

namespace Touresta.API.DTOs.Admin
{
    
    public class AdminHelperReviewResponse
    {
        public string Id { get; set; } = string.Empty;
        public string HelperId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? NationalIdPhoto { get; set; }
        public string? CriminalRecordFile { get; set; }

        public string ApprovalStatus { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }

        public bool IsBanned { get; set; }
        public string? BanReason { get; set; }
        public DateTime? BannedAt { get; set; }

        public bool IsSuspended { get; set; }
        public string? SuspensionReason { get; set; }
        public DateTime? SuspendedAt { get; set; }

        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }

       
        public string UserEmail { get; set; } = string.Empty;
        public string? UserPhone { get; set; }

      
        public DrugTestResponse? CurrentDrugTest { get; set; }


        public List<HelperLanguageResponse> Languages { get; set; } = new();

    
        public CarResponse? Car { get; set; }

       
        public List<CertificateResponse> Certificates { get; set; } = new();
    }
}