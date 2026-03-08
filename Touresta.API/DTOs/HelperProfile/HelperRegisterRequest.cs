using System.ComponentModel.DataAnnotations;
using Touresta.API.Enums;

namespace Touresta.API.DTOs.HelperProfile
{
    /// <summary>Request to register as a helper.</summary>
    public class HelperRegisterRequest
    {
        /// <summary>Full legal name matching National ID.</summary>
        /// <example>Ahmed Mohamed Ali</example>
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>Gender.</summary>
        /// <example>Male</example>
        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        /// <summary>Date of birth.</summary>
        /// <example>1995-06-15</example>
        [Required(ErrorMessage = "Birth date is required.")]
        public DateTime BirthDate { get; set; }
    }
}
