namespace Touresta.API.DTOs.HelperProfile
{
    /// <summary>Helper profile snapshot.</summary>
    public class HelperProfileResponse
    {
        public string HelperId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? NationalIdPhoto { get; set; }
        public string? CriminalRecordFile { get; set; }
        public bool HasCar { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
