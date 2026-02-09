namespace Touresta.API.DTOs.Admin
{
    /// <summary>Lightweight helper item for admin pending queue.</summary>
    public class AdminHelperListItem
    {
        public int Id { get; set; }
        public string HelperId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasDrugTest { get; set; }
        public int LanguageCount { get; set; }
    }
}
