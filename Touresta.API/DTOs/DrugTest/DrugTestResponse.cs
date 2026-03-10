namespace RAFIQ.API.DTOs.DrugTest
{
    /// <summary>Drug test record details.</summary>
    public class DrugTestResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}
