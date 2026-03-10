using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAFIQ.API.DTOs.Admin;
using RAFIQ.API.DTOs.Car;
using RAFIQ.API.DTOs.Certificates;
using RAFIQ.API.DTOs.Common;
using RAFIQ.API.DTOs.DrugTest;
using RAFIQ.API.DTOs.Languages;
using RAFIQ.API.Enums;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;
using RAFIQ.API.Services.Implementations;
using RAFIQ.API.Services.Interfaces;

namespace RAFIQ.API.Controllers
{
    /// <summary>
    /// Admin endpoints for reviewing, moderating, and managing helpers.
    /// Includes onboarding review, approval decisions, operational actions,
    /// search, filtering, and moderation history actions.
    /// </summary>
    [ApiController]
    [Route("api/admin/helpers")]
    [Produces("application/json")]
    [Tags("Admin Helper Management")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminHelperController : ControllerBase
    {
        private readonly IHelperRepository _helperRepo;
        private readonly IHelperService _helperService;
        private readonly IAdminAuditLogRepository _auditLogRepo;

        public AdminHelperController(
            IHelperRepository helperRepo,
            IHelperService helperService,
            IAdminAuditLogRepository auditLogRepo)
        {
            _helperRepo = helperRepo;
            _helperService = helperService;
            _auditLogRepo = auditLogRepo;
        }

        /// <summary>
        /// Extract current admin ID from JWT claims.
        /// </summary>
        /// <returns>The current admin database ID.</returns>
        private string GetCurrentAdminId()
        {
            return User.FindFirst("id")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Write an admin action into the audit log.
        /// </summary>
        private async Task LogAdminAction(string adminId, string action, string targetType, string targetId, string? details)
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

        /// <summary>
        /// Get helpers awaiting admin review.
        /// </summary>
        /// <remarks>
        /// Returns helpers currently in Pending or UnderReview states.
        /// Useful for the admin review queue page.
        /// </remarks>
        /// <response code="200">Pending helpers retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet("pending")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPendingHelpers()
        {
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
                LanguageCount = h.Languages.Count(l => l.IsVerified),
                IsApproved = h.IsApproved,
                IsActive = h.IsActive,
                IsBanned = h.IsBanned,
                IsSuspended = h.IsSuspended
            }).ToList();

            return Ok(new
            {
                message = "Pending helpers retrieved",
                data
            });
        }

        /// <summary>
        /// Get helpers with filtering, search, and pagination.
        /// </summary>
        /// <param name="query">Admin helper query filters and paging options.</param>
        /// <remarks>
        /// Supports search by helper name, helper ID, or user email,
        /// plus filtering by approval status, active state, banned state,
        /// and suspended state.
        /// </remarks>
        /// <response code="200">Helpers retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetHelpers([FromQuery] AdminHelpersQueryRequest query)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

            var result = await _helperRepo.GetHelpersForAdminPagedAsync(
                query.Search,
                query.ApprovalStatus,
                query.IsApproved,
                query.IsActive,
                query.IsBanned,
                query.IsSuspended,
                page,
                pageSize);

            var items = result.Items.Select(h => new AdminHelperListItem
            {
                Id = h.Id,
                HelperId = h.HelperId,
                FullName = h.FullName,
                UserEmail = h.User.Email,
                ApprovalStatus = h.ApprovalStatus.ToString(),
                CreatedAt = h.CreatedAt,
                HasDrugTest = h.DrugTests.Any(dt => dt.IsCurrent),
                LanguageCount = h.Languages.Count(l => l.IsVerified),
                IsApproved = h.IsApproved,
                IsActive = h.IsActive,
                IsBanned = h.IsBanned,
                IsSuspended = h.IsSuspended
            }).ToList();

            var totalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);

            var response = new PagedResponse<AdminHelperListItem>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };

            return Ok(new
            {
                message = "Helpers retrieved successfully.",
                data = response
            });
        }

        /// <summary>
        /// Get full helper review details.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetHelperForReview(string id)
        {
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
                CurrentDrugTest = currentDrugTest != null
                    ? HelperService.MapToDrugTestResponse(currentDrugTest)
                    : null,
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

            return Ok(new
            {
                message = "Helper review data retrieved successfully.",
                data
            });
        }

        /// <summary>
        /// Mark a helper application as under review.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        [HttpPost("{id}/mark-under-review")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> MarkUnderReview(string id)
        {
            var adminId = GetCurrentAdminId();

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            if (helper.ApprovalStatus != ApprovalStatus.Pending)
                return BadRequest(new { message = "Only pending helpers can be marked as under review." });

            helper.ApprovalStatus = ApprovalStatus.UnderReview;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "MarkUnderReview", "Helper", helper.Id, null);

            return Ok(new
            {
                message = "Helper marked as under review successfully.",
                action = "helper_under_review"
            });
        }

        /// <summary>
        /// Approve a helper application.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        [HttpPost("{id}/approve")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ApproveHelper(string id)
        {
            var adminId = GetCurrentAdminId();

            var helper = await _helperService.GetHelperByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            if (helper.ApprovalStatus == ApprovalStatus.Approved)
                return BadRequest(new { message = "Helper is already approved." });

            var eligibility = _helperService.CheckEligibility(helper, helper.User);

            var blockingReasons = eligibility.BlockingReasons
                .Where(r =>
                    r != "Helper is not approved by admin" &&
                    r != "Helper account is not active")
                .ToList();

            helper.IsApproved = true;
            helper.ApprovalStatus = ApprovalStatus.Approved;
            helper.RejectionReason = null;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            if (!helper.IsBanned && !helper.IsSuspended && blockingReasons.Count == 0)
                helper.IsActive = true;
            else
                helper.IsActive = false;

            foreach (var cert in helper.Certificates)
                cert.IsVerified = true;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "ApproveHelper", "Helper", helper.Id, null);

            return Ok(new
            {
                message = helper.IsActive
                    ? "Helper approved and activated successfully."
                    : "Helper approved successfully, but not activated due to missing eligibility requirements.",
                action = "helper_approved",
                activeNow = helper.IsActive,
                blockingReasons
            });
        }

        /// <summary>
        /// Reject a helper application.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <param name="request">Rejection reason.</param>
        [HttpPost("{id}/reject")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RejectHelper(string id, [FromBody] AdminReviewActionRequest request)
        {
            var adminId = GetCurrentAdminId();

            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A reason is required for rejection." });

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            helper.IsApproved = false;
            helper.IsActive = false;
            helper.ApprovalStatus = ApprovalStatus.Rejected;
            helper.RejectionReason = request.Reason;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "RejectHelper", "Helper", helper.Id, request.Reason);

            return Ok(new
            {
                message = "Helper rejected successfully.",
                action = "helper_rejected"
            });
        }

        /// <summary>
        /// Request changes on a helper application.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <param name="request">Required changes description.</param>
        [HttpPost("{id}/request-changes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RequestChanges(string id, [FromBody] AdminReviewActionRequest request)
        {
            var adminId = GetCurrentAdminId();

            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A description of required changes is required." });

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            helper.IsApproved = false;
            helper.IsActive = false;
            helper.ApprovalStatus = ApprovalStatus.ChangesRequested;
            helper.RejectionReason = request.Reason;
            helper.ReviewedAt = DateTime.UtcNow;
            helper.ReviewedByAdminId = adminId;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "RequestChanges", "Helper", helper.Id, request.Reason);

            return Ok(new
            {
                message = "Changes requested successfully.",
                action = "changes_requested"
            });
        }

        /// <summary>
        /// Ban a helper account.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <param name="request">Ban reason.</param>
        [HttpPost("{id}/ban")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BanHelper(string id, [FromBody] AdminReviewActionRequest request)
        {
            var adminId = GetCurrentAdminId();

            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A reason is required for banning." });

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            if (helper.IsBanned)
                return BadRequest(new { message = "Helper is already banned." });

            helper.IsBanned = true;
            helper.BanReason = request.Reason;
            helper.BannedAt = DateTime.UtcNow;
            helper.BannedByAdminId = adminId;
            helper.IsActive = false;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "BanHelper", "Helper", helper.Id, request.Reason);

            return Ok(new
            {
                message = "Helper banned successfully.",
                action = "helper_banned"
            });
        }

        /// <summary>
        /// Remove a helper ban.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        [HttpPost("{id}/unban")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UnbanHelper(string id)
        {
            var adminId = GetCurrentAdminId();

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            if (!helper.IsBanned)
                return BadRequest(new { message = "Helper is not banned." });

            helper.IsBanned = false;
            helper.BanReason = null;
            helper.BannedAt = null;
            helper.BannedByAdminId = null;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "UnbanHelper", "Helper", helper.Id, null);

            return Ok(new
            {
                message = "Helper unbanned successfully.",
                action = "helper_unbanned"
            });
        }

        /// <summary>
        /// Suspend a helper account.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        /// <param name="request">Suspension reason.</param>
        [HttpPost("{id}/suspend")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SuspendHelper(string id, [FromBody] AdminReviewActionRequest request)
        {
            var adminId = GetCurrentAdminId();

            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(new { message = "A reason is required for suspension." });

            var helper = await _helperRepo.GetByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            if (helper.IsSuspended)
                return BadRequest(new { message = "Helper is already suspended." });

            helper.IsSuspended = true;
            helper.SuspensionReason = request.Reason;
            helper.SuspendedAt = DateTime.UtcNow;
            helper.SuspendedByAdminId = adminId;
            helper.IsActive = false;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "SuspendHelper", "Helper", helper.Id, request.Reason);

            return Ok(new
            {
                message = "Helper suspended successfully.",
                action = "helper_suspended"
            });
        }

        /// <summary>
        /// Activate a helper account.
        /// </summary>
        /// <param name="id">Helper database ID.</param>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ActivateHelper(string id)
        {
            var adminId = GetCurrentAdminId();

            var helper = await _helperService.GetHelperByIdAsync(id);
            if (helper == null)
                return NotFound(new { message = "Helper not found." });

            helper.IsSuspended = false;
            helper.SuspensionReason = null;
            helper.SuspendedAt = null;
            helper.SuspendedByAdminId = null;

            var eligibility = _helperService.CheckEligibility(helper, helper.User);
            var blockingReasons = eligibility.BlockingReasons
                .Where(r => r != "Helper account is not active")
                .ToList();

            if (helper.IsBanned)
            {
                blockingReasons.Add("Helper is banned.");
            }

            if (helper.IsSuspended)
            {
                blockingReasons.Add("Helper is suspended.");
            }

            if (blockingReasons.Count > 0)
            {
                await _helperRepo.SaveChangesAsync();

                return BadRequest(new
                {
                    message = "Helper cannot be activated due to blocking conditions.",
                    blockingReasons
                });
            }

            helper.IsActive = true;
            helper.UpdatedAt = DateTime.UtcNow;

            await _helperRepo.SaveChangesAsync();
            await LogAdminAction(adminId, "ActivateHelper", "Helper", helper.Id, null);

            return Ok(new
            {
                message = "Helper activated successfully.",
                action = "helper_activated"
            });
        }
    }
}
