using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs.Admin;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Admin endpoints for internal notes on helpers.
    /// </summary>
    [ApiController]
    [Route("api/admin/helpers/{helperId}/notes")]
    [Produces("application/json")]
    [Tags("Admin Notes")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminNotesController : ControllerBase
    {
        private readonly IAdminNoteRepository _noteRepo;
        private readonly IHelperRepository _helperRepo;
        private readonly IAdminAuditLogRepository _auditRepo;

        public AdminNotesController(
            IAdminNoteRepository noteRepo,
            IHelperRepository helperRepo,
            IAdminAuditLogRepository auditRepo)
        {
            _noteRepo = noteRepo;
            _helperRepo = helperRepo;
            _auditRepo = auditRepo;
        }

        private string GetAdminId()
        {
            return User.FindFirst("id")?.Value ?? string.Empty;
        }

        private async Task LogAction(string action, string targetId, string? details = null)
        {
            _auditRepo.Add(new AdminAuditLog
            {
                AdminId = GetAdminId(),
                Action = action,
                TargetType = "Helper",
                TargetId = targetId,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _auditRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Get internal notes for a helper.
        /// </summary>
        /// <param name="helperId">Helper database ID.</param>
        /// <response code="200">Notes retrieved successfully.</response>
        /// <response code="404">Helper not found.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetNotes(string helperId)
        {
            var helper = await _helperRepo.GetByIdAsync(helperId);
            if (helper == null) return NotFound(new { message = "Helper not found." });

            var data = _noteRepo.Query()
                .Where(n => n.HelperId == helperId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new AdminNoteResponse
                {
                    Id = n.Id,
                    HelperId = n.HelperId,
                    AdminId = n.AdminId,
                    Note = n.Note,
                    CreatedAt = n.CreatedAt
                })
                .ToList();

            return Ok(new { message = "Notes retrieved successfully.", data });
        }

        /// <summary>
        /// Add a new internal note for a helper.
        /// </summary>
        /// <param name="helperId">Helper database ID.</param>
        /// <param name="request">Note content.</param>
        /// <response code="200">Note added successfully.</response>
        /// <response code="400">Note text is missing.</response>
        /// <response code="404">Helper not found.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> AddNote(string helperId, [FromBody] CreateAdminNoteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Note))
                return BadRequest(new { message = "Note is required." });

            var helper = await _helperRepo.GetByIdAsync(helperId);
            if (helper == null) return NotFound(new { message = "Helper not found." });

            var note = new AdminNote
            {
                HelperId = helperId,
                AdminId = GetAdminId(),
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            _noteRepo.Add(note);
            await _noteRepo.SaveChangesAsync();
            await LogAction("AddAdminNote", helperId, request.Note);

            return Ok(new { message = "Note added successfully." });
        }
    }
}
