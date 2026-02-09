namespace Touresta.API.DTOs.DrugTest
{
    /// <summary>Drug test record details.</summary>
    public class DrugTestResponse
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}
