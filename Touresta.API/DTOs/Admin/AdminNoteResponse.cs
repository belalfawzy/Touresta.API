namespace Touresta.API.DTOs.Admin
{
    public class AdminNoteResponse
    {
        public int Id { get; set; }
        public int HelperId { get; set; }
        public int AdminId { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}