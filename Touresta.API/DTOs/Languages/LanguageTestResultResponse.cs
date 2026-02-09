namespace Touresta.API.DTOs.Languages
{
    /// <summary>Result of a language proficiency test attempt.</summary>
    public class LanguageTestResultResponse
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public decimal AiScore { get; set; }
        public string Level { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public int AttemptsUsedThisMonth { get; set; }
        public int MaxAttemptsPerMonth { get; set; } = 3;
        public DateTime? NextRetryAvailableAt { get; set; }
    }
}
