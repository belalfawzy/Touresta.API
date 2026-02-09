using Touresta.API.Enums;

namespace Touresta.API.DTOs.HelperProfile
{
    /// <summary>Request to register as a helper.</summary>
    public class HelperRegisterRequest
    {
        /// <summary>Full legal name matching National ID.</summary>
        /// <example>Ahmed Mohamed Ali</example>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Gender.</summary>
        /// <example>Male</example>
        public Gender Gender { get; set; }

        /// <summary>Date of birth.</summary>
        /// <example>1995-06-15</example>
        public DateTime BirthDate { get; set; }
    }
}
