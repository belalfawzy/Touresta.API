using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs.Car;
using Touresta.API.DTOs.Certificates;
using Touresta.API.DTOs.HelperProfile;
using Touresta.API.DTOs.Languages;
using Touresta.API.Services.Interfaces;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Helper onboarding endpoints. All endpoints require a valid JWT token (authenticated User).
    /// The helper onboarding pipeline: Register → Profile → Documents → Languages → Drug Test → (Car) → (Certificates) → Admin Review.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Helper Onboarding")]
    [Authorize]
    public class HelperController : ControllerBase
    {
        private readonly IHelperService _helperService;

        public HelperController(IHelperService helperService)
        {
            _helperService = helperService;
        }

        /// <summary>Extracts the current User ID from JWT claims.</summary>
        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("id")?.Value;
            return int.Parse(idClaim!);
        }

        // ─── Registration ────────────────────────────────────────────

        /// <summary>
        /// Creates a Helper profile linked to the authenticated User.
        /// Requires a verified User account. Arabic is auto-added as a verified native language.
        /// </summary>
        /// <param name="request">Helper registration data.</param>
        /// <response code="200">Helper profile created successfully.</response>
        /// <response code="400">User not verified or helper already exists.</response>
        /// <response code="401">Not authenticated.</response>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Register([FromBody] HelperRegisterRequest request)
        {
            var userId = GetCurrentUserId();
            var (success, message, data) = await _helperService.RegisterHelperAsync(userId, request);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, action = "continue_onboarding", data });
        }

        // ─── Profile ─────────────────────────────────────────────────

        /// <summary>
        /// Updates helper profile fields and uploads documents (national ID, criminal record).
        /// If the helper had ChangesRequested status, re-submission resets to Pending.
        /// </summary>
        /// <param name="request">Profile fields and document files to update.</param>
        /// <response code="200">Profile updated successfully.</response>
        /// <response code="400">Validation or upload error.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpPut("profile")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProfile(
            [FromForm] HelperProfileUpdateRequest request)
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message, data) = await _helperService.UpdateProfileAsync(
                helper.Id, request, request.NationalIdPhoto, request.CriminalRecordFile);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, action = "profile_updated", data });
        }

        // ─── Status ──────────────────────────────────────────────────

        /// <summary>
        /// Returns the helper's onboarding progress including computed status, completed steps,
        /// and a list of missing steps needed before admin review.
        /// </summary>
        /// <response code="200">Onboarding status retrieved.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpGet("status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStatus()
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var data = _helperService.GetStatus(helper, helper.User);
            return Ok(new { message = "Onboarding status retrieved", data });
        }

        // ─── Drug Test ───────────────────────────────────────────────

        /// <summary>
        /// Uploads a drug test document. Expiry is automatically set to 6 months from upload date.
        /// Previous drug tests are marked as not current. If the helper was deactivated due to
        /// drug test expiry, uploading a new one auto-reactivates them (if still admin-approved).
        /// </summary>
        /// <param name="drugTestFile">Certified drug test result (JPG/PNG/PDF, max 10MB).</param>
        /// <response code="200">Drug test uploaded. Valid for 6 months.</response>
        /// <response code="400">Validation or upload error.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpPost("drug-test")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UploadDrugTest(IFormFile drugTestFile)
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message, data) = await _helperService.UploadDrugTestAsync(helper.Id, drugTestFile);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, action = "drug_test_uploaded", data });
        }

        // ─── Car ─────────────────────────────────────────────────────

        /// <summary>
        /// Adds or updates car information with license documents.
        /// Sets HasCar = true on the helper profile.
        /// </summary>
        /// <param name="request">Car details and license files.</param>
        /// <response code="200">Car information saved.</response>
        /// <response code="400">Validation or upload error.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpPost("car")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddOrUpdateCar(
            [FromForm] CarRequest request)
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message, data) = await _helperService.AddOrUpdateCarAsync(
                helper.Id, request, request.CarLicenseFile, request.PersonalLicenseFile);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, action = "car_saved", data });
        }

        /// <summary>
        /// Removes car information and deletes associated files. Sets HasCar = false.
        /// </summary>
        /// <response code="200">Car removed.</response>
        /// <response code="400">No car registered.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpDelete("car")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveCar()
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message) = await _helperService.RemoveCarAsync(helper.Id);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // ─── Certificates ────────────────────────────────────────────

        /// <summary>
        /// Uploads a professional certificate (Tour Guide, Archaeology, History, or Other).
        /// Certificates are optional but improve search ranking and trust score.
        /// Admin verifies authenticity during review.
        /// </summary>
        /// <param name="request">Certificate details and file.</param>
        /// <response code="200">Certificate uploaded.</response>
        /// <response code="400">Validation or upload error.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpPost("certificates")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddCertificate(
            [FromForm] CertificateUploadRequest request)
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message, data) = await _helperService.AddCertificateAsync(
                helper.Id, request, request.CertificateFile);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, action = "certificate_uploaded", data });
        }

        /// <summary>
        /// Removes a certificate by ID. Verifies the certificate belongs to the current helper.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <response code="200">Certificate removed.</response>
        /// <response code="400">Certificate not found or not owned by this helper.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpDelete("certificates/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveCertificate(int id)
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message) = await _helperService.RemoveCertificateAsync(helper.Id, id);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // ─── Languages ───────────────────────────────────────────────

        /// <summary>
        /// Returns the list of supported languages with a flag indicating if the helper already has each one.
        /// Arabic is always auto-added as a verified native language during registration.
        /// </summary>
        /// <response code="200">Available languages retrieved.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpGet("languages")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAvailableLanguages()
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var data = await _helperService.GetAvailableLanguagesAsync(helper.Id);
            return Ok(new { message = "Available languages", data });
        }

        /// <summary>
        /// Takes a language proficiency test. Answers are evaluated by AI.
        /// Rate limits: max 3 attempts per language per month, 24-hour cooldown between attempts.
        /// Arabic cannot be tested (auto-verified at Native level).
        /// Passing threshold: score >= 60 (Intermediate or above).
        /// </summary>
        /// <param name="code">ISO 639-1 language code (e.g., "en", "fr").</param>
        /// <param name="request">Test answers.</param>
        /// <response code="200">Test evaluated. Check 'passed' field for result.</response>
        /// <response code="400">Validation error or rate limit exceeded.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpPost("languages/{code}/test")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TakeLanguageTest(string code, [FromBody] LanguageTestSubmitRequest request)
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var (success, message, data) = await _helperService.TakeLanguageTestAsync(helper.Id, code, request);

            if (!success)
                return BadRequest(new { message, data });

            return Ok(new { message, data });
        }

        /// <summary>
        /// Returns the helper's registered languages with proficiency levels, AI scores, and verification status.
        /// </summary>
        /// <response code="200">Languages retrieved.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpGet("languages/my")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMyLanguages()
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var data = await _helperService.GetMyLanguagesAsync(helper.Id);
            return Ok(new { message = "Your languages", data });
        }

        // ─── Eligibility ─────────────────────────────────────────────

        /// <summary>
        /// Checks if the helper meets all 5 activation conditions for booking eligibility:
        /// (1) User email verified, (2) Admin approved, (3) Account active,
        /// (4) Valid drug test on file, (5) At least one verified language.
        /// Returns specific blocking reasons if not eligible.
        /// </summary>
        /// <response code="200">Eligibility check complete.</response>
        /// <response code="404">No helper profile found.</response>
        [HttpGet("eligibility")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckEligibility()
        {
            var helper = await _helperService.GetHelperByUserIdAsync(GetCurrentUserId());
            if (helper == null)
                return NotFound(new { message = "No helper profile found. Please register first." });

            var data = _helperService.CheckEligibility(helper, helper.User);
            return Ok(new { message = "Eligibility check complete", data });
        }
    }
}
