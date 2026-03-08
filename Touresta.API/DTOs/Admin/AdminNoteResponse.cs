namespace Touresta.API.DTOs.Admin
{
    public class AdminNoteResponse
    {
        public string Id { get; set; } = string.Empty;
        public string HelperId { get; set; } = string.Empty;
        public string AdminId { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}