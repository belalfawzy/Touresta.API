using RAFIQ.API.DTOs.Car;
using RAFIQ.API.DTOs.Certificates;
using RAFIQ.API.DTOs.DrugTest;
using RAFIQ.API.DTOs.HelperProfile;
using RAFIQ.API.DTOs.Languages;
using RAFIQ.API.Models;

namespace RAFIQ.API.Services.Interfaces
{
    public interface IHelperService
    {
        // Registration & Profile
        Task<(bool Success, string Message, HelperProfileResponse? Data)> RegisterHelperAsync(string userId, HelperRegisterRequest request);
        Task<(bool Success, string Message, HelperProfileResponse? Data)> UpdateProfileAsync(string helperId, HelperProfileUpdateRequest request, IFormFile? nationalIdPhoto, IFormFile? criminalRecordFile);
        HelperStatusResponse GetStatus(Helper helper, User user);

        // Drug Test
        Task<(bool Success, string Message, DrugTestResponse? Data)> UploadDrugTestAsync(string helperId, IFormFile drugTestFile);

        // Car
        Task<(bool Success, string Message, CarResponse? Data)> AddOrUpdateCarAsync(string helperId, CarRequest request, IFormFile carLicenseFile, IFormFile personalLicenseFile);
        Task<(bool Success, string Message)> RemoveCarAsync(string helperId);

        // Certificates
        Task<(bool Success, string Message, CertificateResponse? Data)> AddCertificateAsync(string helperId, CertificateUploadRequest request, IFormFile certificateFile);
        Task<(bool Success, string Message)> RemoveCertificateAsync(string helperId, string certificateId);

        // Languages
        Task<List<LanguageListItem>> GetAvailableLanguagesAsync(string helperId);
        Task<(bool Success, string Message, LanguageTestResultResponse? Data)> TakeLanguageTestAsync(string helperId, string languageCode, LanguageTestSubmitRequest request);
        Task<List<HelperLanguageResponse>> GetMyLanguagesAsync(string helperId);

        // Eligibility
        HelperEligibilityResponse CheckEligibility(Helper helper, User user);

        // Loading
        Task<Helper?> GetHelperByUserIdAsync(string userId);
        Task<Helper?> GetHelperByIdAsync(string helperId);
    }
}
