namespace Touresta.API.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? Action { get; set; }
        public string? Token { get; set; }
    }
}
