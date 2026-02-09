namespace Touresta.API.DTOs.HelperProfile
{
    /// <summary>Comprehensive onboarding progress snapshot.</summary>
    public class HelperStatusResponse
    {
        public string HelperId { get; set; } = string.Empty;
        public string ComputedStatus { get; set; } = string.Empty;
        public bool ProfileComplete { get; set; }
        public bool NationalIdUploaded { get; set; }
        public bool CriminalRecordUploaded { get; set; }
        public bool DrugTestUploaded { get; set; }
        public bool DrugTestValid { get; set; }
        public DateTime? DrugTestExpiry { get; set; }
        public int LanguagesVerified { get; set; }
        public bool HasCar { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public List<string> MissingSteps { get; set; } = new();
    }
}
