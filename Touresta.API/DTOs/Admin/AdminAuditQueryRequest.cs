namespace Touresta.API.DTOs.Admin
{
    public class AdminAuditQueryRequest
    {
        public string? TargetType { get; set; }
        public int? TargetId { get; set; }
        public int? AdminId { get; set; }
        public string? Action { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}