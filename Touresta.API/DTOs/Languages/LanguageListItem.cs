namespace Touresta.API.DTOs.Languages
{
    /// <summary>Available language for helper testing.</summary>
    public class LanguageListItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool AlreadyAdded { get; set; }
    }
}
