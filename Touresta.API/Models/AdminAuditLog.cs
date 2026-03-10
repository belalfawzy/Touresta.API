namespace RAFIQ.API.Models
{
    public class AdminAuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string AdminId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
    }
}
