namespace RAFIQ.API.DTOs.Certificates
{
    /// <summary>Certificate details response.</summary>
    public class CertificateResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
