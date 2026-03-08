namespace Touresta.API.Models
{
    public class AdminNote
    {
        public int Id { get; set; }

        public int HelperId { get; set; }
        public Helper Helper { get; set; } = null!;

        public int AdminId { get; set; }
        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}