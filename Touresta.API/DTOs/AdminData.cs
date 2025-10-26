namespace Touresta.API.DTOs
{
    public class AdminData
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Type { get; set; } = "admin";
    }
}
