namespace Touresta.API.DTOs.Admin
{
    public class HelperReportListItemResponse
    {
        public string Id { get; set; } = string.Empty;
        public string HelperId { get; set; } = string.Empty;
        public string HelperName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public bool IsResolved { get; set; }
        public string? ResolutionNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}