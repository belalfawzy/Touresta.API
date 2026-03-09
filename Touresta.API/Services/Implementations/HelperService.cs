using Touresta.API.DTOs.Car;
using Touresta.API.DTOs.Certificates;
using Touresta.API.DTOs.DrugTest;
using Touresta.API.DTOs.HelperProfile;
using Touresta.API.DTOs.Languages;
using Touresta.API.Enums;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;
using Touresta.API.Services.Interfaces;

namespace Touresta.API.Services.Implementations
{
    public class HelperService : IHelperService
    {
        private readonly IUserRepository _userRepo;
        private readonly IHelperRepository _helperRepo;
        private readonly IHelperLanguageRepository _helperLanguageRepo;
        private readonly ICertificateRepository _certificateRepo;
        private readonly ICarRepository _carRepo;
        private readonly IDrugTestRepository _drugTestRepo;
        private readonly ILanguageTestRepository _languageTestRepo;
        private readonly ICloudinaryService _cloudinary;
        private readonly ILanguageEvaluationService _languageEval;
        private readonly ILogger<HelperService> _logger;

        private static readonly List<(string Code, string Name)> SupportedLanguages = new()
        {
            ("ar", "Arabic"),
            ("en", "English"),
            ("fr", "French"),
            ("de", "German"),
            ("es", "Spanish"),
            ("it", "Italian"),
            ("pt", "Portuguese"),
            ("ru", "Russian"),
            ("zh", "Chinese"),
            ("ja", "Japanese"),
            ("ko", "Korean"),
            ("tr", "Turkish")
        };

        public HelperService(
            IUserRepository userRepo,
            IHelperRepository helperRepo,
            IHelperLanguageRepository helperLanguageRepo,
            ICertificateRepository certificateRepo,
            ICarRepository carRepo,
            IDrugTestRepository drugTestRepo,
            ILanguageTestRepository languageTestRepo,
            ICloudinaryService cloudinary,
            ILanguageEvaluationService languageEval,
            ILogger<HelperService> logger)
        {
            _userRepo = userRepo;
            _helperRepo = helperRepo;
            _helperLanguageRepo = helperLanguageRepo;
            _certificateRepo = certificateRepo;
            _carRepo = carRepo;
            _drugTestRepo = drugTestRepo;
            _languageTestRepo = languageTestRepo;
            _cloudinary = cloudinary;
            _languageEval = languageEval;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, HelperProfileResponse? Data)> RegisterHelperAsync(
            string userId, HelperRegisterRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return (false, "User not found.", null);
            if (!user.IsVerified) return (false, "User email must be verified before registering as a helper.", null);
            if (await _helperRepo.ExistsByUserIdAsync(userId)) return (false, "A helper profile already exists for this user.", null);

            var helper = new Helper
            {
                HelperId = Guid.NewGuid().ToString(),
                UserId = userId,
                FullName = request.FullName,
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                IsActive = false,
                IsApproved = false,
                ApprovalStatus = ApprovalStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _helperRepo.Add(helper);

            _helperLanguageRepo.Add(new HelperLanguage
            {
                Helper = helper,
                LanguageCode = "ar",
                LanguageName = "Arabic",
                Level = LanguageLevel.Native,
                IsVerified = true,
                TestAttempts = 0
            });

            await _helperRepo.SaveChangesAsync();
            return (true, "Helper profile created successfully.", MapToProfileResponse(helper));
        }

        public async Task<(bool Success, string Message, HelperProfileResponse? Data)> UpdateProfileAsync(
            string helperId, HelperProfileUpdateRequest request, IFormFile? nationalIdPhoto, IFormFile? criminalRecordFile)
        {
            var helper = await _helperRepo.GetByIdAsync(helperId);
            if (helper == null) return (false, "Helper not found.", null);

            if (request.FullName != null) helper.FullName = request.FullName;
            if (request.Gender.HasValue) helper.Gender = request.Gender.Value;
            if (request.BirthDate.HasValue) helper.BirthDate = request.BirthDate.Value;

            if (nationalIdPhoto != null)
            {
                var (success, url, msg) = await _cloudinary.UploadFileAsync(
                    nationalIdPhoto, $"helpers/{helperId}/national-id", maxSizeMb: 5);

                if (!success) return (false, $"National ID upload failed: {msg}", null);
                helper.NationalIdPhoto = url;
            }

            if (criminalRecordFile != null)
            {
                var (success, url, msg) = await _cloudinary.UploadFileAsync(
                    criminalRecordFile, $"helpers/{helperId}/criminal-record", maxSizeMb: 10);

                if (!success) return (false, $"Criminal record upload failed: {msg}", null);
                helper.CriminalRecordFile = url;
            }

            if (helper.ApprovalStatus == ApprovalStatus.ChangesRequested)
            {
                helper.ApprovalStatus = ApprovalStatus.Pending;
                helper.RejectionReason = null;
            }

            helper.UpdatedAt = DateTime.UtcNow;
            await _helperRepo.SaveChangesAsync();

            return (true, "Profile updated successfully.", MapToProfileResponse(helper));
        }

        public HelperStatusResponse GetStatus(Helper helper, User user)
        {
            var currentDrugTest = helper.DrugTests?.FirstOrDefault(dt => dt.IsCurrent);
            var drugTestValid = currentDrugTest != null && currentDrugTest.ExpiryDate > DateTime.UtcNow;
            var languagesVerified = helper.Languages?.Count(l => l.IsVerified) ?? 0;
            var computedStatus = ComputeStatus(helper, user, currentDrugTest);

            var missingSteps = new List<string>();

            if (!user.IsVerified) missingSteps.Add("Verify your email address");
            if (string.IsNullOrEmpty(helper.FullName)) missingSteps.Add("Complete your profile (full name)");
            if (string.IsNullOrEmpty(helper.NationalIdPhoto)) missingSteps.Add("Upload national ID photo");
            if (string.IsNullOrEmpty(helper.CriminalRecordFile)) missingSteps.Add("Upload criminal record certificate (Fish & Tashbih directed to Touresta)");
            if (currentDrugTest == null) missingSteps.Add("Upload drug test document");
            else if (!drugTestValid) missingSteps.Add("Upload a new drug test (current one expired)");
            if (languagesVerified == 0) missingSteps.Add("Verify at least one language (Arabic is auto-verified)");

            if (helper.IsBanned) missingSteps.Add("Your account is banned by admin");
            if (helper.IsSuspended) missingSteps.Add("Your account is suspended by admin");

            return new HelperStatusResponse
            {
                HelperId = helper.HelperId,
                ComputedStatus = computedStatus,
                ProfileComplete = !string.IsNullOrEmpty(helper.FullName),
                NationalIdUploaded = !string.IsNullOrEmpty(helper.NationalIdPhoto),
                CriminalRecordUploaded = !string.IsNullOrEmpty(helper.CriminalRecordFile),
                DrugTestUploaded = currentDrugTest != null,
                DrugTestValid = drugTestValid,
                DrugTestExpiry = currentDrugTest?.ExpiryDate,
                LanguagesVerified = languagesVerified,
                HasCar = helper.HasCar,
                IsApproved = helper.IsApproved,
                IsActive = helper.IsActive,
                IsBanned = helper.IsBanned,
                IsSuspended = helper.IsSuspended,
                ApprovalStatus = helper.ApprovalStatus.ToString(),
                RejectionReason = helper.RejectionReason,
                BanReason = helper.BanReason,
                SuspensionReason = helper.SuspensionReason,
                MissingSteps = missingSteps
            };
        }

        public async Task<(bool Success, string Message, DrugTestResponse? Data)> UploadDrugTestAsync(
            string helperId, IFormFile drugTestFile)
        {
            var helper = await _helperRepo.GetByIdWithDrugTestsAsync(helperId);
            if (helper == null) return (false, "Helper not found.", null);

            var (success, url, msg) = await _cloudinary.UploadFileAsync(
                drugTestFile, $"helpers/{helperId}/drug-test", maxSizeMb: 10);

            if (!success) return (false, $"Drug test upload failed: {msg}", null);

            foreach (var existing in helper.DrugTests.Where(dt => dt.IsCurrent))
                existing.IsCurrent = false;

            var drugTest = new DrugTest
            {
                HelperId = helperId,
                FilePath = url,
                UploadedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsCurrent = true
            };

            _drugTestRepo.Add(drugTest);

            if (helper.IsApproved && !helper.IsActive && !helper.IsBanned && !helper.IsSuspended)
                helper.IsActive = true;

            helper.UpdatedAt = DateTime.UtcNow;
            await _helperRepo.SaveChangesAsync();

            _logger.LogInformation("Drug test uploaded for HelperId {HelperId}, expires {Expiry}", helperId, drugTest.ExpiryDate);
            return (true, "Drug test uploaded successfully. Valid for 6 months.", MapToDrugTestResponse(drugTest));
        }

        public async Task<(bool Success, string Message, CarResponse? Data)> AddOrUpdateCarAsync(
            string helperId, CarRequest request, IFormFile carLicenseFile, IFormFile personalLicenseFile)
        {
            var helper = await _helperRepo.GetByIdWithCarAsync(helperId);
            if (helper == null) return (false, "Helper not found.", null);

            var plateExists = await _carRepo.LicensePlateExistsAsync(request.LicensePlate, helperId);
            if (plateExists) return (false, "License plate already registered to another helper.", null);

            var (clSuccess, clUrl, clMsg) = await _cloudinary.UploadFileAsync(
                carLicenseFile, $"helpers/{helperId}/car", maxSizeMb: 10);
            if (!clSuccess) return (false, $"Car license upload failed: {clMsg}", null);

            var (plSuccess, plUrl, plMsg) = await _cloudinary.UploadFileAsync(
                personalLicenseFile, $"helpers/{helperId}/car", maxSizeMb: 10);
            if (!plSuccess) return (false, $"Personal license upload failed: {plMsg}", null);

            if (helper.Car != null)
            {
                helper.Car.Brand = request.Brand;
                helper.Car.Model = request.Model;
                helper.Car.Color = request.Color;
                helper.Car.LicensePlate = request.LicensePlate;
                helper.Car.EnergyType = request.EnergyType;
                helper.Car.Type = request.Type;
                helper.Car.CarLicenseFile = clUrl;
                helper.Car.PersonalLicenseFile = plUrl;
            }
            else
            {
                var car = new Car
                {
                    HelperId = helperId,
                    Brand = request.Brand,
                    Model = request.Model,
                    Color = request.Color,
                    LicensePlate = request.LicensePlate,
                    EnergyType = request.EnergyType,
                    Type = request.Type,
                    CarLicenseFile = clUrl,
                    PersonalLicenseFile = plUrl
                };

                _carRepo.Add(car);
                helper.Car = car;
            }

            helper.HasCar = true;
            helper.UpdatedAt = DateTime.UtcNow;
            await _helperRepo.SaveChangesAsync();

            return (true, "Car information saved successfully.", MapToCarResponse(helper.Car!));
        }

        public async Task<(bool Success, string Message)> RemoveCarAsync(string helperId)
        {
            var helper = await _helperRepo.GetByIdWithCarAsync(helperId);
            if (helper == null) return (false, "Helper not found.");
            if (helper.Car == null) return (false, "No car registered for this helper.");

            var carLicenseId = CloudinaryService.ExtractPublicIdFromUrl(helper.Car.CarLicenseFile);
            var personalLicenseId = CloudinaryService.ExtractPublicIdFromUrl(helper.Car.PersonalLicenseFile);

            if (carLicenseId != null) await _cloudinary.DeleteFileAsync(carLicenseId);
            if (personalLicenseId != null) await _cloudinary.DeleteFileAsync(personalLicenseId);

            _carRepo.Remove(helper.Car);
            helper.HasCar = false;
            helper.UpdatedAt = DateTime.UtcNow;
            await _helperRepo.SaveChangesAsync();

            return (true, "Car removed successfully.");
        }

        public async Task<(bool Success, string Message, CertificateResponse? Data)> AddCertificateAsync(
            string helperId, CertificateUploadRequest request, IFormFile certificateFile)
        {
            var helper = await _helperRepo.GetByIdAsync(helperId);
            if (helper == null) return (false, "Helper not found.", null);

            var (success, url, msg) = await _cloudinary.UploadFileAsync(
                certificateFile, $"helpers/{helperId}/certificates", maxSizeMb: 10);

            if (!success) return (false, $"Certificate upload failed: {msg}", null);

            var certificate = new Certificate
            {
                HelperId = helperId,
                Name = request.Name,
                FilePath = url,
                Type = request.Type,
                IsVerified = false,
                UploadedAt = DateTime.UtcNow
            };

            _certificateRepo.Add(certificate);
            helper.UpdatedAt = DateTime.UtcNow;
            await _helperRepo.SaveChangesAsync();

            return (true, "Certificate uploaded successfully.", MapToCertificateResponse(certificate));
        }

        public async Task<(bool Success, string Message)> RemoveCertificateAsync(string helperId, string certificateId)
        {
            var certificate = await _certificateRepo.GetByIdAndHelperIdAsync(certificateId, helperId);
            if (certificate == null) return (false, "Certificate not found or does not belong to this helper.");

            var publicId = CloudinaryService.ExtractPublicIdFromUrl(certificate.FilePath);
            if (publicId != null) await _cloudinary.DeleteFileAsync(publicId);

            _certificateRepo.Remove(certificate);
            await _certificateRepo.SaveChangesAsync();

            return (true, "Certificate removed successfully.");
        }

        public async Task<List<LanguageListItem>> GetAvailableLanguagesAsync(string helperId)
        {
            var existingCodes = await _helperLanguageRepo.GetLanguageCodesByHelperIdAsync(helperId);

            return SupportedLanguages.Select(lang => new LanguageListItem
            {
                Code = lang.Code,
                Name = lang.Name,
                AlreadyAdded = existingCodes.Contains(lang.Code)
            }).ToList();
        }

        public async Task<(bool Success, string Message, LanguageTestResultResponse? Data)> TakeLanguageTestAsync(
            string helperId, string languageCode, LanguageTestSubmitRequest request)
        {
            var lang = SupportedLanguages.FirstOrDefault(l => l.Code == languageCode);
            if (lang == default) return (false, $"Language code '{languageCode}' is not supported.", null);

            if (languageCode == "ar")
                return (false, "Arabic is auto-verified at Native level. No test required.", null);

            if (request.Answers == null || request.Answers.Count == 0)
                return (false, "No answers provided.", null);

            var helperLang = await _helperLanguageRepo.GetByHelperAndCodeAsync(helperId, languageCode);
            if (helperLang == null)
            {
                helperLang = new HelperLanguage
                {
                    HelperId = helperId,
                    LanguageCode = languageCode,
                    LanguageName = lang.Name,
                    Level = LanguageLevel.None,
                    IsVerified = false,
                    TestAttempts = 0
                };

                _helperLanguageRepo.Add(helperLang);
                await _helperLanguageRepo.SaveChangesAsync();
            }

            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var attemptsThisMonth = helperLang.TestHistory?.Count(t => t.TakenAt >= monthStart) ?? 0;

            if (attemptsThisMonth >= 3)
                return (false, "Maximum 3 test attempts per language per month reached. Try again next month.", null);

            if (helperLang.LastTestedAt.HasValue)
            {
                var nextRetry = helperLang.LastTestedAt.Value.AddHours(24);
                if (DateTime.UtcNow < nextRetry)
                {
                    return (false, $"Please wait until {nextRetry:yyyy-MM-dd HH:mm} UTC before retrying.",
                        new LanguageTestResultResponse
                        {
                            LanguageCode = languageCode,
                            LanguageName = lang.Name,
                            AttemptsUsedThisMonth = attemptsThisMonth,
                            NextRetryAvailableAt = nextRetry
                        });
                }
            }

            var evalResult = await _languageEval.EvaluateAsync(languageCode, request.Answers);

            var test = new LanguageTest
            {
                HelperLanguageId = helperLang.Id,
                AiScore = evalResult.Score,
                AiLevel = evalResult.Level,
                Passed = evalResult.Passed,
                TakenAt = DateTime.UtcNow
            };

            _languageTestRepo.Add(test);

            helperLang.AiScore = evalResult.Score;
            helperLang.Level = evalResult.Level;
            helperLang.TestAttempts++;
            helperLang.LastTestedAt = DateTime.UtcNow;

            if (evalResult.Passed)
                helperLang.IsVerified = true;

            await _helperLanguageRepo.SaveChangesAsync();

            return (true,
                evalResult.Passed
                    ? $"Congratulations! You passed the {lang.Name} test with a score of {evalResult.Score}."
                    : $"You did not pass the {lang.Name} test. Score: {evalResult.Score}. You can retry after 24 hours.",
                new LanguageTestResultResponse
                {
                    LanguageCode = languageCode,
                    LanguageName = lang.Name,
                    AiScore = evalResult.Score,
                    Level = evalResult.Level.ToString(),
                    Passed = evalResult.Passed,
                    AttemptsUsedThisMonth = attemptsThisMonth + 1,
                    NextRetryAvailableAt = evalResult.Passed ? null : DateTime.UtcNow.AddHours(24)
                });
        }

        public async Task<List<HelperLanguageResponse>> GetMyLanguagesAsync(string helperId)
        {
            var languages = await _helperLanguageRepo.GetByHelperIdAsync(helperId);

            return languages.Select(hl => new HelperLanguageResponse
            {
                Id = hl.Id,
                LanguageCode = hl.LanguageCode,
                LanguageName = hl.LanguageName,
                Level = hl.Level.ToString(),
                AiScore = hl.AiScore,
                TestAttempts = hl.TestAttempts,
                LastTestedAt = hl.LastTestedAt,
                IsVerified = hl.IsVerified
            }).ToList();
        }

        public HelperEligibilityResponse CheckEligibility(Helper helper, User user)
        {
            var currentDrugTest = helper.DrugTests?.FirstOrDefault(dt => dt.IsCurrent);
            var hasValidDrugTest = currentDrugTest != null && currentDrugTest.ExpiryDate > DateTime.UtcNow;
            var hasVerifiedLang = helper.Languages?.Any(l => l.IsVerified) ?? false;

            var reasons = new List<string>();

            if (!user.IsVerified) reasons.Add("User email is not verified");
            if (!helper.IsApproved) reasons.Add("Helper is not approved by admin");
            if (!helper.IsActive) reasons.Add("Helper account is not active");
            if (helper.IsBanned) reasons.Add("Helper account is banned by admin");
            if (helper.IsSuspended) reasons.Add("Helper account is suspended by admin");
            if (!hasValidDrugTest) reasons.Add("No valid drug test on file (expired or missing)");
            if (!hasVerifiedLang) reasons.Add("No verified language on profile");

            return new HelperEligibilityResponse
            {
                IsEligible = reasons.Count == 0,
                UserVerified = user.IsVerified,
                IsApproved = helper.IsApproved,
                IsActive = helper.IsActive,
                HasValidDrugTest = hasValidDrugTest,
                HasVerifiedLanguage = hasVerifiedLang,
                IsBanned = helper.IsBanned,
                IsSuspended = helper.IsSuspended,
                BlockingReasons = reasons
            };
        }

        public async Task<Helper?> GetHelperByUserIdAsync(string userId) =>
            await _helperRepo.GetByUserIdWithFullIncludesAsync(userId);

        public async Task<Helper?> GetHelperByIdAsync(string helperId) =>
            await _helperRepo.GetByIdWithFullIncludesAsync(helperId);

        private static string ComputeStatus(Helper helper, User user, Models.DrugTest? currentDrugTest)
        {
            if (!user.IsVerified) return "Unverified";
            if (helper.IsBanned) return "Banned";
            if (helper.IsSuspended) return "Suspended";
            if (helper.ApprovalStatus == ApprovalStatus.Rejected) return "Rejected";
            if (helper.ApprovalStatus == ApprovalStatus.ChangesRequested) return "ChangesRequested";
            if (helper.ApprovalStatus == ApprovalStatus.Pending) return "Pending";
            if (helper.ApprovalStatus == ApprovalStatus.UnderReview) return "UnderReview";
            if (!helper.IsApproved) return "Pending";
            if (currentDrugTest == null || currentDrugTest.ExpiryDate < DateTime.UtcNow) return "Suspended";
            if (!helper.IsActive) return "Inactive";
            return "Active";
        }

        private static HelperProfileResponse MapToProfileResponse(Helper h) => new()
        {
            HelperId = h.HelperId,
            FullName = h.FullName,
            Gender = h.Gender.ToString(),
            BirthDate = h.BirthDate,
            ProfileImageUrl = h.ProfileImageUrl,
            NationalIdPhoto = h.NationalIdPhoto,
            CriminalRecordFile = h.CriminalRecordFile,
            HasCar = h.HasCar,
            IsActive = h.IsActive,
            IsApproved = h.IsApproved,
            ApprovalStatus = h.ApprovalStatus.ToString(),
            RejectionReason = h.RejectionReason,
            IsBanned = h.IsBanned,
            BanReason = h.BanReason,
            IsSuspended = h.IsSuspended,
            SuspensionReason = h.SuspensionReason,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        };

        public static DrugTestResponse MapToDrugTestResponse(Models.DrugTest dt) => new()
        {
            Id = dt.Id,
            FilePath = dt.FilePath,
            UploadedAt = dt.UploadedAt,
            ExpiryDate = dt.ExpiryDate,
            IsCurrent = dt.IsCurrent
        };

        public static CarResponse MapToCarResponse(Models.Car c) => new()
        {
            Id = c.Id,
            Brand = c.Brand,
            Model = c.Model,
            Color = c.Color.ToString(),
            LicensePlate = c.LicensePlate,
            EnergyType = c.EnergyType.ToString(),
            Type = c.Type.ToString(),
            CarLicenseFile = c.CarLicenseFile,
            PersonalLicenseFile = c.PersonalLicenseFile
        };

        public static CertificateResponse MapToCertificateResponse(Certificate cert) => new()
        {
            Id = cert.Id,
            Name = cert.Name,
            FilePath = cert.FilePath,
            Type = cert.Type.ToString(),
            IsVerified = cert.IsVerified,
            UploadedAt = cert.UploadedAt
        };
    }
}
