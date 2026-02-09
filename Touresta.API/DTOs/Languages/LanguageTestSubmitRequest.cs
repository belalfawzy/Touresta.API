namespace Touresta.API.DTOs.Languages
{
    /// <summary>Submit answers for a language proficiency test.</summary>
    public class LanguageTestSubmitRequest
    {
        /// <summary>List of question-answer pairs.</summary>
        public List<LanguageTestAnswer> Answers { get; set; } = new();
    }

    /// <summary>A single question-answer pair.</summary>
    public class LanguageTestAnswer
    {
        /// <summary>Question identifier.</summary>
        /// <example>1</example>
        public int QuestionId { get; set; }

        /// <summary>Helper's answer text.</summary>
        /// <example>The Pyramids of Giza are located in Cairo.</example>
        public string Answer { get; set; } = string.Empty;
    }
}
