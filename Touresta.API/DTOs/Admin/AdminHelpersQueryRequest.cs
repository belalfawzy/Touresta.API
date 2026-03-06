namespace Touresta.API.DTOs.Admin
{
    public class AdminHelpersQueryRequest
    {
        public string? Search { get; set; }
        public string? ApprovalStatus { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBanned { get; set; }
        public bool? IsSuspended { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}