namespace RAFIQ.API.DTOs.Admin
{
    public class AdminDashboardStatsResponse
    {
        public int TotalHelpers { get; set; }
        public int PendingHelpers { get; set; }
        public int ApprovedHelpers { get; set; }
        public int ActiveHelpers { get; set; }
        public int SuspendedHelpers { get; set; }
        public int BannedHelpers { get; set; }
    }
}