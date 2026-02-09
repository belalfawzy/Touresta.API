using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public CertificateType Type { get; set; } = CertificateType.Other;
        public bool IsVerified { get; set; } = false;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int HelperId { get; set; }
        public Helper Helper { get; set; } = null!;
    }
}
