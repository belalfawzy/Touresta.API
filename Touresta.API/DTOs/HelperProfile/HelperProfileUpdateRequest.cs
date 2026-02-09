using Microsoft.AspNetCore.Http;
using Touresta.API.Enums;

namespace Touresta.API.DTOs.HelperProfile
{
    /// <summary>Request to update helper profile fields.</summary>
    public class HelperProfileUpdateRequest
    {
        /// <summary>Full legal name. Only updated if provided.</summary>
        /// <example>Ahmed Mohamed Ali</example>
        public string? FullName { get; set; }

        /// <summary>Gender. Only updated if provided.</summary>
        /// <example>Male</example>
        public Gender? Gender { get; set; }

        /// <summary>Date of birth. Only updated if provided.</summary>
        /// <example>1995-06-15</example>
        public DateTime? BirthDate { get; set; }

        /// <summary>National ID photo (JPG/PNG/PDF, max 5MB).</summary>
        public IFormFile? NationalIdPhoto { get; set; }

        /// <summary>Criminal record certificate - Fish and Tashbih directed to Touresta (JPG/PNG/PDF, max 10MB).</summary>
        public IFormFile? CriminalRecordFile { get; set; }
    }
}
