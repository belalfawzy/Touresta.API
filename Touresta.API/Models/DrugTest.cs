namespace Touresta.API.Models
{
    public class DrugTest
    {
        public int Id { get; set; }

        public int HelperId { get; set; }
        public Helper Helper { get; set; } = null!;

        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public bool IsCurrent { get; set; } = true;
    }
}
