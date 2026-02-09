using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Touresta.API.Services.Interfaces;

namespace Touresta.API.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".pdf"
        };

        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "application/pdf"
        };

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Uploads a file to Cloudinary after validating extension, MIME type, and size.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="folder">Cloudinary folder path (e.g. "helpers/5/national-id").</param>
        /// <param name="maxSizeMb">Maximum allowed file size in megabytes.</param>
        /// <returns>Tuple of success flag, secure URL (or empty), and message.</returns>
        public async Task<(bool Success, string Url, string Message)> UploadFileAsync(
            IFormFile file, string folder, int maxSizeMb = 5)
        {
            // Validate extension
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                return (false, "", $"File type '{extension}' is not allowed. Accepted: JPG, PNG, PDF.");

            // Validate MIME type
            if (!AllowedMimeTypes.Contains(file.ContentType))
                return (false, "", $"MIME type '{file.ContentType}' is not allowed.");

            // Validate file size
            var maxBytes = maxSizeMb * 1024L * 1024L;
            if (file.Length > maxBytes)
                return (false, "", $"File size exceeds {maxSizeMb}MB limit.");

            if (file.Length == 0)
                return (false, "", "File is empty.");

            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                PublicId = Guid.NewGuid().ToString()
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                return (false, "", $"Upload failed: {result.Error.Message}");

            return (true, result.SecureUrl.ToString(), "Upload successful");
        }

        /// <summary>
        /// Deletes a file from Cloudinary by its public ID.
        /// </summary>
        public async Task<bool> DeleteFileAsync(string publicId)
        {
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            return result.Result == "ok";
        }

        /// <summary>
        /// Extracts the Cloudinary public ID from a full secure URL.
        /// </summary>
        public static string? ExtractPublicIdFromUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // Cloudinary URLs format: https://res.cloudinary.com/{cloud}/raw/upload/v{version}/{folder}/{publicId}.{ext}
            // We need the folder/publicId part (without extension)
            try
            {
                var uri = new Uri(url);
                var path = uri.AbsolutePath;
                // Find the segment after "/upload/v{version}/"
                var uploadIndex = path.IndexOf("/upload/", StringComparison.Ordinal);
                if (uploadIndex < 0) return null;

                var afterUpload = path[(uploadIndex + 8)..]; // skip "/upload/"
                // Skip version segment "v12345/"
                var slashIndex = afterUpload.IndexOf('/');
                if (slashIndex < 0) return null;

                var publicIdWithExt = afterUpload[(slashIndex + 1)..];
                // Remove extension
                var dotIndex = publicIdWithExt.LastIndexOf('.');
                return dotIndex > 0 ? publicIdWithExt[..dotIndex] : publicIdWithExt;
            }
            catch
            {
                return null;
            }
        }
    }
}
