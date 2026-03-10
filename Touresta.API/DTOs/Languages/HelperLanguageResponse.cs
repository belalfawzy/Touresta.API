namespace RAFIQ.API.DTOs.Languages
{
    /// <summary>Helper's language proficiency details.</summary>
    public class HelperLanguageResponse
    {
        public string Id { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public decimal? AiScore { get; set; }
        public int TestAttempts { get; set; }
        public DateTime? LastTestedAt { get; set; }
        public bool IsVerified { get; set; }
    }
}
