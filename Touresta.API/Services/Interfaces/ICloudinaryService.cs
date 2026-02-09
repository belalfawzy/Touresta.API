using Microsoft.AspNetCore.Http;

namespace Touresta.API.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(bool Success, string Url, string Message)> UploadFileAsync(IFormFile file, string folder, int maxSizeMb = 5);
        Task<bool> DeleteFileAsync(string publicId);
    }
}
