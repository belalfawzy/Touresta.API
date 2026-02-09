using Touresta.API.DTOs.Car;
using Touresta.API.DTOs.Certificates;
using Touresta.API.DTOs.DrugTest;
using Touresta.API.DTOs.Languages;

namespace Touresta.API.DTOs.Admin
{
    /// <summary>Full helper review data package for admin.</summary>
    public class AdminHelperReviewResponse
    {
        public int Id { get; set; }
        public string HelperId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? NationalIdPhoto { get; set; }
        public string? CriminalRecordFile { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }

        // Linked User info
        public string UserEmail { get; set; } = string.Empty;
        public string? UserPhone { get; set; }

        // Drug test
        public DrugTestResponse? CurrentDrugTest { get; set; }

        // Languages
        public List<HelperLanguageResponse> Languages { get; set; } = new();

        // Car
        public CarResponse? Car { get; set; }

        // Certificates
        public List<CertificateResponse> Certificates { get; set; } = new();
    }
}
