namespace Touresta.API.Models
{
    public class HelperReport
    {
        public int Id { get; set; }

        public int HelperId { get; set; }
        public Helper Helper { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Reason { get; set; } = string.Empty;
        public string? Details { get; set; }

        public bool IsResolved { get; set; } = false;
        public string? ResolutionNote { get; set; }
        public int? ResolvedByAdminId { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}