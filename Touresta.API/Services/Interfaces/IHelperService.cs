using Touresta.API.DTOs.Car;
using Touresta.API.DTOs.Certificates;
using Touresta.API.DTOs.DrugTest;
using Touresta.API.DTOs.HelperProfile;
using Touresta.API.DTOs.Languages;
using Touresta.API.Models;

namespace Touresta.API.Services.Interfaces
{
    public interface IHelperService
    {
        // Registration & Profile
        Task<(bool Success, string Message, HelperProfileResponse? Data)> RegisterHelperAsync(int userId, HelperRegisterRequest request);
        Task<(bool Success, string Message, HelperProfileResponse? Data)> UpdateProfileAsync(int helperId, HelperProfileUpdateRequest request, IFormFile? nationalIdPhoto, IFormFile? criminalRecordFile);
        HelperStatusResponse GetStatus(Helper helper, User user);

        // Drug Test
        Task<(bool Success, string Message, DrugTestResponse? Data)> UploadDrugTestAsync(int helperId, IFormFile drugTestFile);

        // Car
        Task<(bool Success, string Message, CarResponse? Data)> AddOrUpdateCarAsync(int helperId, CarRequest request, IFormFile carLicenseFile, IFormFile personalLicenseFile);
        Task<(bool Success, string Message)> RemoveCarAsync(int helperId);

        // Certificates
        Task<(bool Success, string Message, CertificateResponse? Data)> AddCertificateAsync(int helperId, CertificateUploadRequest request, IFormFile certificateFile);
        Task<(bool Success, string Message)> RemoveCertificateAsync(int helperId, int certificateId);

        // Languages
        Task<List<LanguageListItem>> GetAvailableLanguagesAsync(int helperId);
        Task<(bool Success, string Message, LanguageTestResultResponse? Data)> TakeLanguageTestAsync(int helperId, string languageCode, LanguageTestSubmitRequest request);
        Task<List<HelperLanguageResponse>> GetMyLanguagesAsync(int helperId);

        // Eligibility
        HelperEligibilityResponse CheckEligibility(Helper helper, User user);

        // Loading
        Task<Helper?> GetHelperByUserIdAsync(int userId);
        Task<Helper?> GetHelperByIdAsync(int helperId);
    }
}
