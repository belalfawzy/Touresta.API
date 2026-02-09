using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class HelperLanguage
    {
        public int Id { get; set; }

        public int HelperId { get; set; }
        public Helper Helper { get; set; } = null!;

        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public LanguageLevel Level { get; set; } = LanguageLevel.None;
        public decimal? AiScore { get; set; }
        public int TestAttempts { get; set; }
        public DateTime? LastTestedAt { get; set; }
        public bool IsVerified { get; set; }

        // Navigation
        public List<LanguageTest> TestHistory { get; set; } = new();
    }
}
