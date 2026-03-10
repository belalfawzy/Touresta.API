using System.ComponentModel.DataAnnotations;

namespace RAFIQ.API.DTOs.Languages
{
    /// <summary>Submit answers for a language proficiency test.</summary>
    public class LanguageTestSubmitRequest
    {
        /// <summary>List of question-answer pairs.</summary>
        [Required(ErrorMessage = "Answers are required.")]
        [MinLength(1, ErrorMessage = "At least one answer must be provided.")]
        public List<LanguageTestAnswer> Answers { get; set; } = new();
    }

    /// <summary>A single question-answer pair.</summary>
    public class LanguageTestAnswer
    {
        /// <summary>Question identifier.</summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Question ID is required.")]
        public int QuestionId { get; set; }

        /// <summary>Helper's answer text.</summary>
        /// <example>The Pyramids of Giza are located in Cairo.</example>
        [Required(ErrorMessage = "Answer is required.")]
        public string Answer { get; set; } = string.Empty;
    }
}
