namespace RAFIQ.API.DTOs.Admin
{
    public class AdminHelperListItem
    {
        public string Id { get; set; } = string.Empty;
        public string HelperId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasDrugTest { get; set; }
        public int LanguageCount { get; set; }

        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public bool IsBanned { get; set; }
        public bool IsSuspended { get; set; }
    }
}