namespace Touresta.API.DTOs.Admin
{
    /// <summary>Admin review action request (reject or request changes).</summary>
    public class AdminReviewActionRequest
    {
        /// <summary>Reason for rejection or description of required changes.</summary>
        /// <example>National ID photo is blurry, please re-upload.</example>
        public string? Reason { get; set; }
    }
}
