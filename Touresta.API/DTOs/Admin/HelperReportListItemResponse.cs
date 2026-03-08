namespace Touresta.API.DTOs.Admin
{
    public class HelperReportListItemResponse
    {
        public int Id { get; set; }
        public int HelperId { get; set; }
        public string HelperName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }
        public bool IsResolved { get; set; }
        public string? ResolutionNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}