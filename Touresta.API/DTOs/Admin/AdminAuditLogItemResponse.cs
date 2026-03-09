namespace Touresta.API.DTOs.Admin
{
    public class AdminAuditLogItemResponse
    {
        public string Id { get; set; } = string.Empty;
        public string AdminId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }
}