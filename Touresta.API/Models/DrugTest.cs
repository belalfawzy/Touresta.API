namespace RAFIQ.API.Models
{
    public class DrugTest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string HelperId { get; set; } = string.Empty;
        public Helper Helper { get; set; } = null!;

        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public bool IsCurrent { get; set; } = true;
    }
}
