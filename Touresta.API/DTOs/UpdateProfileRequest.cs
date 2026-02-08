namespace Touresta.API.DTOs
{
    /// <summary>
    /// Request to update user profile fields. Only provided fields will be updated.
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>The user's unique ID (required).</summary>
        /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
        public string UserId { get; set; } = string.Empty;

        /// <summary>New display name (optional).</summary>
        /// <example>JohnUpdated</example>
        public string? UserName { get; set; }

        /// <summary>New phone number (optional).</summary>
        /// <example>+201234567890</example>
        public string? PhoneNumber { get; set; }

        /// <summary>New country (optional).</summary>
        /// <example>Egypt</example>
        public string? Country { get; set; }

        /// <summary>New gender (optional).</summary>
        /// <example>Male</example>
        public string? Gender { get; set; }

        /// <summary>New birth date (optional).</summary>
        /// <example>1995-06-15</example>
        public DateTime? BirthDate { get; set; }
    }
}
