namespace Touresta.API.DTOs.Languages
{
    /// <summary>Helper's language proficiency details.</summary>
    public class HelperLanguageResponse
    {
        public int Id { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public decimal? AiScore { get; set; }
        public int TestAttempts { get; set; }
        public DateTime? LastTestedAt { get; set; }
        public bool IsVerified { get; set; }
    }
}
