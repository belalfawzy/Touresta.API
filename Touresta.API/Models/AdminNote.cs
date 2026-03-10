namespace RAFIQ.API.Models
{
    public class AdminNote
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string HelperId { get; set; } = string.Empty;
        public Helper Helper { get; set; } = null!;

        public string AdminId { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
