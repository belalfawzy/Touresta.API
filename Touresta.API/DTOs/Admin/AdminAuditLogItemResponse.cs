namespace Touresta.API.DTOs.Admin
{
    public class AdminAuditLogItemResponse
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public int TargetId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }
}