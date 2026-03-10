namespace RAFIQ.API.DTOs.Admin
{
    public class AdminAuditQueryRequest
    {
        public string? TargetType { get; set; }
        public string? TargetId { get; set; }
        public string? AdminId { get; set; }
        public string? Action { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}