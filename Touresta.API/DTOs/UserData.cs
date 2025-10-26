namespace Touresta.API.DTOs
{
    public class UserData
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string Type { get; set; } = "user";
    }
}
