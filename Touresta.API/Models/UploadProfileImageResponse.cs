namespace Touresta.API.Models
{
    public class UploadProfileImageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
