using Touresta.API.Enums;

namespace Touresta.API.DTOs.Certificates
{
    /// <summary>Request to upload a professional certificate.</summary>
    public class CertificateUploadRequest
    {
        /// <summary>Certificate name or title.</summary>
        /// <example>Tour Guide License - Cairo</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>Certificate category.</summary>
        /// <example>TourGuide</example>
        public CertificateType Type { get; set; }
    }
}
