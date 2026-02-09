using Touresta.API.DTOs.Languages;
using Touresta.API.Enums;
using Touresta.API.Services.Interfaces;

namespace Touresta.API.Services.Implementations
{
    /// <summary>
    /// Stub implementation that returns randomized scores.
    /// Replace with a real AI provider (OpenAI, Gemini, etc.) in production.
    /// </summary>
    public class StubLanguageEvaluationService : ILanguageEvaluationService
    {
        public async Task<LanguageEvaluationResult> EvaluateAsync(string languageCode, List<LanguageTestAnswer> answers)
        {
            // Simulate AI processing time
            await Task.Delay(500);

            var random = new Random();
            var score = Math.Round((decimal)(random.Next(4000, 9600) / 100.0), 2);

            var level = score switch
            {
                >= 90m => LanguageLevel.Native,
                >= 75m => LanguageLevel.Advanced,
                >= 60m => LanguageLevel.Intermediate,
                >= 40m => LanguageLevel.Beginner,
                _ => LanguageLevel.None
            };

            var passed = score >= 60m;

            return new LanguageEvaluationResult(score, level, passed);
        }
    }
}
