using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class LanguageTest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public string HelperLanguageId { get; set; } = string.Empty;
        public HelperLanguage HelperLanguage { get; set; } = null!;

        public decimal AiScore { get; set; }
        public LanguageLevel AiLevel { get; set; }
        public bool Passed { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    }
}
