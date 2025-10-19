namespace Touresta.API.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string FilePath { get; set; } 
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int HelperId { get; set; }
        public Helper Helper { get; set; }
    }
}
