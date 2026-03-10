using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RAFIQ.API.Enums;

namespace RAFIQ.API.DTOs.Certificates
{
    /// <summary>Request to upload a professional certificate.</summary>
    public class CertificateUploadRequest
    {
        /// <summary>Certificate name or title.</summary>
        /// <example>Tour Guide License - Cairo</example>
        [Required(ErrorMessage = "Certificate name is required.")]
        [StringLength(200, ErrorMessage = "Certificate name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Certificate category.</summary>
        /// <example>TourGuide</example>
        [Required(ErrorMessage = "Certificate type is required.")]
        public CertificateType Type { get; set; }

        /// <summary>Certificate document (JPG/PNG/PDF, max 10MB).</summary>
        [Required(ErrorMessage = "Certificate file is required.")]
        public IFormFile CertificateFile { get; set; } = null!;
    }
}
