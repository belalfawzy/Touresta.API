using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class Certificate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public CertificateType Type { get; set; } = CertificateType.Other;
        public bool IsVerified { get; set; } = false;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string HelperId { get; set; } = string.Empty;
        public Helper Helper { get; set; } = null!;
    }
}
