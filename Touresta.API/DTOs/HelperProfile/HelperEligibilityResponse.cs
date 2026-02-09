namespace Touresta.API.DTOs.HelperProfile
{
    /// <summary>Helper booking eligibility check result.</summary>
    public class HelperEligibilityResponse
    {
        public bool IsEligible { get; set; }
        public bool UserVerified { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public bool HasValidDrugTest { get; set; }
        public bool HasVerifiedLanguage { get; set; }
        public List<string> BlockingReasons { get; set; } = new();
    }
}
