using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs.Admin;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Admin dashboard endpoints for statistics and recent activity.
    /// </summary>
    [ApiController]
    [Route("api/admin/dashboard")]
    [Produces("application/json")]
    [Tags("Admin Dashboard")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IHelperRepository _helperRepo;
        private readonly IAdminAuditLogRepository _auditRepo;

        public AdminDashboardController(
            IHelperRepository helperRepo,
            IAdminAuditLogRepository auditRepo)
        {
            _helperRepo = helperRepo;
            _auditRepo = auditRepo;
        }

        /// <summary>
        /// Get dashboard statistics.
        /// </summary>
        /// <remarks>
        /// Returns high-level counters for helper management including total helpers,
        /// pending reviews, approved helpers, active helpers, suspended helpers, and banned helpers.
        /// </remarks>
        /// <response code="200">Dashboard stats retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet("stats")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = new AdminDashboardStatsResponse
            {
                TotalHelpers = await _helperRepo.CountAllAsync(),
                PendingHelpers = await _helperRepo.CountPendingAsync(),
                ApprovedHelpers = await _helperRepo.CountApprovedAsync(),
                ActiveHelpers = await _helperRepo.CountActiveAsync(),
                SuspendedHelpers = await _helperRepo.CountSuspendedAsync(),
                BannedHelpers = await _helperRepo.CountBannedAsync()
            };

            return Ok(new
            {
                message = "Dashboard stats retrieved successfully.",
                data = stats
            });
        }

        /// <summary>
        /// Get recent helper registrations.
        /// </summary>
        /// <remarks>
        /// Returns the latest helper accounts created in the system for quick review on the admin dashboard.
        /// </remarks>
        /// <response code="200">Recent helpers retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet("recent-helpers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetRecentHelpers()
        {
            var helpers = await _helperRepo.GetRecentHelpersAsync(10);

            var data = helpers.Select(h => new AdminRecentHelperResponse
            {
                Id = h.Id,
                HelperId = h.HelperId,
                FullName = h.FullName,
                Email = h.User.Email,
                ApprovalStatus = h.ApprovalStatus.ToString(),
                CreatedAt = h.CreatedAt
            });

            return Ok(new
            {
                message = "Recent helpers retrieved.",
                data
            });
        }

        /// <summary>
        /// Get recent admin actions.
        /// </summary>
        /// <remarks>
        /// Returns the latest recorded admin actions from the audit log,
        /// such as approvals, bans, suspensions, and admin management actions.
        /// </remarks>
        /// <response code="200">Recent admin actions retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet("recent-actions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetRecentAdminActions()
        {
            var actions = _auditRepo.Query()
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .Select(a => new
                {
                    a.AdminId,
                    a.Action,
                    a.TargetType,
                    a.TargetId,
                    a.Timestamp
                })
                .ToList();

            return Ok(new
            {
                message = "Recent admin actions retrieved.",
                data = actions
            });
        }
    }
}