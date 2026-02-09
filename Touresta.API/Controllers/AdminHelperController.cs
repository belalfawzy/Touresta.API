using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs.Admin;
using Touresta.API.DTOs.Car;
using Touresta.API.DTOs.Certificates;
using Touresta.API.DTOs.DrugTest;
using Touresta.API.DTOs.Languages;
using Touresta.API.Enums;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;
using Touresta.API.Services.Implementations;
using Touresta.API.Services.Interfaces;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Admin endpoints for reviewing and managing helper applications.
    /// All endpoints require a valid admin JWT token (type = "admin").
    /// Every action is logged in the AdminAuditLog for traceability.
    /// </summary>
    [ApiController]
    [Route("api/admin/helpers")]
    [Produces("application/json")]
    [Tags("Admin Helper Management")]
    [Authorize]
    public class AdminHelperController : ControllerBase
    {
        private readonly IHelperRepository _helperRepo;
        private readonly IHelperService _helperService;
        private readonly IAdminAuditLogRepository _auditLogRepo;

        public AdminHelperController(IHelperRepository helperRepo, IHelperService helperService, IAdminAuditLogRepository auditLogRepo)
        {
            _helperRepo = helperRepo;
            _helperService = helperService;
            _auditLogRepo = auditLogRepo;
        }

        /// <summary>Validates the current token is an admin and returns the admin ID.</summary>
        private (bool IsValid, int AdminId) ValidateAdminClaim()
        {
            var typeClaim = User.FindFirst("type")?.Value;
            if (typeClaim != "admin") return (false, 0);

            var idClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(idClaim, out var adminId)) return (false, 0);

            return (true, adminId);
        }

        /// <summary>Logs an admin action to the audit trail.</summary>
        private async Task LogAdminAction(int adminId, string action, string targetType, int targetId, string? details)
        {
            _auditLogRepo.Add(new AdminAuditLog
            {
                AdminId = adminId,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
            await _auditLogRepo.SaveChangesAsync();
        }

        // ─── List Pending ────────────────────────────────────────────

        /// <summary>
        /// Returns the queue of helper applications awaiting admin review.
        /// Includes helpers with Pending or UnderReview status.
        /// </summary>
        /// <response code="200">Pending helpers retrieved.</response>
        /// <response code="401">Not authenticated or not an admin.</response>
        [HttpGet("pending")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPendingHelpers()
        {
            var (isValid, _) = ValidateAdminClaim();
            if (!isValid) return Unauthorized(new { message = "Admin access required." });

            var helpers = await _helperRepo.GetPendingHelpersAsync();

            var data = helpers.Select(h => new AdminHelperListItem
            {
                Id = h.Id,
                HelperId = h.HelperId,
                FullName = h.FullName,
                UserEmail = h.User.Email,
                ApprovalStatus = h.ApprovalStatus.ToString(),
                CreatedAt = h.CreatedAt,
                HasDrugTest = h.DrugTests.Any(dt => dt.IsCurrent),
                LanguageCount = h.Languages.Count(l => l.IsVerified)
            }).ToList();

            return Ok(new { message = "Pending helpers retrieved", data });
        }

        // ─── Review Detail ───────────────────────────────────────────

        /// <summary>
        /// Returns the full data package for a helper application review.
        /// Includes personal info, linked user data, documents, drug test, languages, car, and certificates.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <response code="200">Helper review data retrieved.</response>
        /// <response code="401">Not authenticated or not an admin.</response>
        /// <response code="404">Helper not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetHelperForReview(int id)
        {
            var (isValid, _) = ValidateAdminClaim();
            if (!isValid) return Unauthorized(new { message = "Admin access required." });

            var helper = await _helperService.GetHelperByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            var currentDrugTest = helper.DrugTests?.FirstOrDefault(dt => dt.IsCurrent);

            var data = new AdminHelperReviewResponse
            {
                Id = helper.Id,
                HelperId = helper.HelperId,
                FullName = helper.FullName,
                Gender = helper.Gender.ToString(),
                BirthDate = helper.BirthDate,
                ProfileImageUrl = helper.ProfileImageUrl,
                NationalIdPhoto = helper.NationalIdPhoto,
                CriminalRecordFile = helper.CriminalRecordFile,
                ApprovalStatus = helper.ApprovalStatus.ToString(),
                RejectionReason = helper.RejectionReason,
                CreatedAt = helper.CreatedAt,
                UserEmail = helper.User.Email,
                UserPhone = helper.User.PhoneNumber,
                CurrentDrugTest = currentDrugTest != null ? HelperService.MapToDrugTestResponse(currentDrugTest) : null,
                Languages = helper.Languages?.Select(l => new HelperLanguageResponse
                {
                    Id = l.Id,
                    LanguageCode = l.LanguageCode,
                    LanguageName = l.LanguageName,
                    Level = l.Level.ToString(),
                    AiScore = l.AiScore,
                    TestAttempts = l.TestAttempts,
                    LastTestedAt = l.LastTestedAt,
                    IsVerified = l.IsVerified
                }).ToList() ?? new(),
                Car = helper.Car != null ? HelperService.MapToCarResponse(helper.Car) : null,
                Certificates = helper.Certificates?.Select(HelperService.MapToCertificateResponse).ToList() ?? new()
            };

            return Ok(new { message = "Helper review data", data });
        }

        // ─── Approve ─────────────────────────────────────────────────

        /// <summary>
        /// Approves a helper application. Sets IsApproved = true, IsActive = true, ApprovalStatus = Approved.
        /// Also marks all certificates as verified. The helper becomes operational.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <response code="200">Helper approved and activated.</response>
        /// <response code="400">Helper cannot be approved in current state.</response>
        /// <response code="401">Not authenticated or not an admin.</response>
        /// <response code="404">Helper not found.</response>
        [HttpPost("{id}/approve")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ApproveHelper(int id)
        {
            var (isValid, adminId) = ValidateAdminClaim();
            if (!isValid) return Unauthorized(new { message = "Admin access required." });

            var helper = await _helperRepo.GetByIdWithCertificatesAsync(id);

            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            if (helper.ApprovalStatus == ApprovalStatus.Approved)
                return BadRequest(new { message = "Helper is already approved." });

            helper.IsApproved = true;
            helper.IsActive = true;
            helper.ApprovalStatus = ApprovalStatus.Approved;
            helper.RejectionReason = null;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            // Mark all certificates as verified on approval
            foreach (var cert in helper.Certificates)
                cert.IsVerified = true;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "ApproveHelper", "Helper", helper.Id, null);

            return Ok(new { message = "Helper approved and activated.", action = "helper_approved" });
        }

        // ─── Reject ──────────────────────────────────────────────────

        /// <summary>
        /// Rejects a helper application. A reason is required.
        /// The helper is notified and remains inactive.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <param name="request">Rejection reason (required).</param>
        /// <response code="200">Helper rejected.</response>
        /// <response code="400">Missing reason or invalid state.</response>
        /// <response code="401">Not authenticated or not an admin.</response>
        /// <response code="404">Helper not found.</response>
        [HttpPost("{id}/reject")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RejectHelper(int id, [FromBody] AdminReviewActionRequest request)
        {
            var (isValid, adminId) = ValidateAdminClaim();
            if (!isValid) return Unauthorized(new { message = "Admin access required." });

            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A reason is required for rejection." });

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            helper.ApprovalStatus = ApprovalStatus.Rejected;
            helper.RejectionReason = request.Reason;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "RejectHelper", "Helper", helper.Id, request.Reason);

            return Ok(new { message = "Helper rejected.", action = "helper_rejected" });
        }

        // ─── Request Changes ─────────────────────────────────────────

        /// <summary>
        /// Requests corrections on a helper application. A description of required changes is required.
        /// The helper can update their data and re-submit, which resets status to Pending.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <param name="request">Description of required changes (required).</param>
        /// <response code="200">Changes requested.</response>
        /// <response code="400">Missing reason or invalid state.</response>
        /// <response code="401">Not authenticated or not an admin.</response>
        /// <response code="404">Helper not found.</response>
        [HttpPost("{id}/request-changes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RequestChanges(int id, [FromBody] AdminReviewActionRequest request)
        {
            var (isValid, adminId) = ValidateAdminClaim();
            if (!isValid) return Unauthorized(new { message = "Admin access required." });

            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A description of required changes is required." });

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            helper.ApprovalStatus = ApprovalStatus.ChangesRequested;
            helper.RejectionReason = request.Reason;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "RequestChanges", "Helper", helper.Id, request.Reason);

            return Ok(new { message = "Changes requested.", action = "changes_requested" });
        }
    }
}
