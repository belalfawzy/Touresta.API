using RAFIQ.API.DTOs.Languages;
using RAFIQ.API.Enums;

namespace RAFIQ.API.Services.Interfaces
{
    /// <summary>
    /// Evaluates a helper's language proficiency based on submitted test answers.
    /// Implement this interface with a real AI provider (OpenAI, Gemini, etc.) in production.
    /// </summary>
    public interface ILanguageEvaluationService
    {
        Task<LanguageEvaluationResult> EvaluateAsync(string languageCode, List<LanguageTestAnswer> answers);
    }

    /// <summary>Result of an AI language evaluation.</summary>
    public record LanguageEvaluationResult(
        decimal Score,
        LanguageLevel Level,
        bool Passed
    );
}
