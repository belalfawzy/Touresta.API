namespace Touresta.API.DTOs.Admin
{
    public class AdminRecentHelperResponse
    {
        public string Id { get; set; } = string.Empty;
        public string HelperId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}