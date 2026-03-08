using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs.Admin;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Admin endpoints for reviewing and resolving helper reports.
    /// </summary>
    [ApiController]
    [Route("api/admin/reports")]
    [Produces("application/json")]
    [Tags("Admin Reports")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminReportsController : ControllerBase
    {
        private readonly IHelperReportRepository _reportRepo;
        private readonly IAdminAuditLogRepository _auditRepo;

        public AdminReportsController(
            IHelperReportRepository reportRepo,
            IAdminAuditLogRepository auditRepo)
        {
            _reportRepo = reportRepo;
            _auditRepo = auditRepo;
        }

        private int GetAdminId()
        {
            var idClaim = User.FindFirst("id")?.Value;
            return int.TryParse(idClaim, out var adminId) ? adminId : 0;
        }

        private async Task LogAction(string action, int targetId, string? details = null)
        {
            _auditRepo.Add(new Touresta.API.Models.AdminAuditLog
            {
                AdminId = GetAdminId(),
                Action = action,
                TargetType = "HelperReport",
                TargetId = targetId,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _auditRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Get helper reports.
        /// </summary>
        /// <param name="isResolved">Optional filter to return only resolved or unresolved reports.</param>
        /// <remarks>
        /// Returns helper reports submitted against drivers/helpers,
        /// including reporter information, reason, details, and resolution state.
        /// </remarks>
        /// <response code="200">Reports retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetReports([FromQuery] bool? isResolved)
        {
            var query = _reportRepo.Query();

            if (isResolved.HasValue)
                query = query.Where(r => r.IsResolved == isResolved.Value);

            var data = query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new HelperReportListItemResponse
                {
                    Id = r.Id,
                    HelperId = r.HelperId,
                    HelperName = r.Helper.FullName,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    Reason = r.Reason,
                    Details = r.Details,
                    IsResolved = r.IsResolved,
                    ResolutionNote = r.ResolutionNote,
                    CreatedAt = r.CreatedAt,
                    ResolvedAt = r.ResolvedAt
                })
                .ToList();

            return Ok(new { message = "Reports retrieved successfully.", data });
        }

        /// <summary>
        /// Resolve a helper report.
        /// </summary>
        /// <param name="id">Report database ID.</param>
        /// <param name="request">Resolution note.</param>
        /// <remarks>
        /// Marks a helper report as resolved and stores the admin resolution note.
        /// </remarks>
        /// <response code="200">Report resolved successfully.</response>
        /// <response code="400">Report already resolved.</response>
        /// <response code="404">Report not found.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpPatch("{id}/resolve")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ResolveReport(int id, [FromBody] ResolveHelperReportRequest request)
        {
            var report = await _reportRepo.GetByIdAsync(id);
            if (report == null) return NotFound(new { message = "Report not found." });

            if (report.IsResolved)
                return BadRequest(new { message = "Report already resolved." });

            report.IsResolved = true;
            report.ResolutionNote = request.ResolutionNote;
            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedByAdminId = GetAdminId();

            await _reportRepo.SaveChangesAsync();
            await LogAction("ResolveHelperReport", report.Id, request.ResolutionNote);

            return Ok(new { message = "Report resolved successfully." });
        }
    }
}