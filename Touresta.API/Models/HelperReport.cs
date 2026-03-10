namespace RAFIQ.API.Models
{
    public class HelperReport
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string HelperId { get; set; } = string.Empty;
        public Helper Helper { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }

        public bool IsResolved { get; set; } = false;
        public string? ResolutionNote { get; set; }
        public string? ResolvedByAdminId { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
