using Touresta.API.Enums;

namespace Touresta.API.Models
{
    public class LanguageTest
    {
        public int Id { get; set; }

        public int HelperLanguageId { get; set; }
        public HelperLanguage HelperLanguage { get; set; } = null!;

        public decimal AiScore { get; set; }
        public LanguageLevel AiLevel { get; set; }
        public bool Passed { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    }
}
