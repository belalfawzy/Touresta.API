using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Touresta.API.DTOs.Admin;
using Touresta.API.Enums;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;
using AdminModel = Touresta.API.Models.Admin;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Super admin endpoints for managing admin accounts.
    /// </summary>
    [ApiController]
    [Route("api/admin/admins")]
    [Produces("application/json")]
    [Tags("Admin Management")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Authorize(Policy = "SuperAdminOnly")]
    public class AdminManagementController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IAdminAuditLogRepository _auditRepo;
        private readonly PasswordHasher<AdminModel> _hasher;

        public AdminManagementController(
            IAdminRepository adminRepo,
            IAdminAuditLogRepository auditRepo)
        {
            _adminRepo = adminRepo;
            _auditRepo = auditRepo;
            _hasher = new PasswordHasher<AdminModel>();
        }

        private int GetCurrentAdminId()
        {
            var idClaim = User.FindFirst("id")?.Value;
            return int.TryParse(idClaim, out var adminId) ? adminId : 0;
        }

        private async Task LogAction(string action, int targetId, string? details = null)
        {
            _auditRepo.Add(new AdminAuditLog
            {
                AdminId = GetCurrentAdminId(),
                Action = action,
                TargetType = "Admin",
                TargetId = targetId,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _auditRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Get all admin accounts.
        /// </summary>
        /// <remarks>
        /// Returns all system admins with their roles, status, and creation dates.
        /// Accessible only by the super admin.
        /// </remarks>
        /// <response code="200">Admins retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Super admin access is required.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _adminRepo.GetAllAsync();

            var data = admins.Select(a => new AdminListItemResponse
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                Role = a.Role.ToString(),
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            });

            return Ok(new { message = "Admins retrieved successfully.", data });
        }

        /// <summary>
        /// Create a new admin account.
        /// </summary>
        /// <param name="request">New admin account data.</param>
        /// <remarks>
        /// Creates a new admin or super admin account.
        /// This endpoint is restricted to the super admin only.
        /// </remarks>
        /// <response code="200">Admin created successfully.</response>
        /// <response code="400">Invalid request or email already exists.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Super admin access is required.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "FullName, Email and Password are required." });
            }

            if (await _adminRepo.ExistsByEmailAsync(request.Email))
                return BadRequest(new { message = "Admin email already exists." });

            if (!Enum.TryParse<Role>(request.Role, true, out var role))
                return BadRequest(new { message = "Invalid role." });

            var admin = new AdminModel
            {
                FullName = request.FullName,
                Email = request.Email,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            admin.PasswordHash = _hasher.HashPassword(admin, request.Password);

            _adminRepo.Add(admin);
            await _adminRepo.SaveChangesAsync();
            await LogAction("CreateAdmin", admin.Id, $"Created admin with role {admin.Role}");

            return Ok(new
            {
                message = "Admin created successfully.",
                data = new AdminListItemResponse
                {
                    Id = admin.Id,
                    FullName = admin.FullName,
                    Email = admin.Email,
                    Role = admin.Role.ToString(),
                    IsActive = admin.IsActive,
                    CreatedAt = admin.CreatedAt
                }
            });
        }

        /// <summary>
        /// Update an admin role.
        /// </summary>
        /// <param name="id">Admin database ID.</param>
        /// <param name="request">New role data.</param>
        /// <remarks>
        /// Changes the role of an existing admin account.
        /// Only the super admin can perform this action.
        /// </remarks>
        /// <response code="200">Admin role updated successfully.</response>
        /// <response code="400">Invalid role.</response>
        /// <response code="404">Admin not found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Super admin access is required.</response>
        [HttpPatch("{id}/role")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateAdminRole(int id, [FromBody] UpdateAdminRoleRequest request)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound(new { message = "Admin not found." });

            if (!Enum.TryParse<Role>(request.Role, true, out var role))
                return BadRequest(new { message = "Invalid role." });

            admin.Role = role;
            await _adminRepo.SaveChangesAsync();
            await LogAction("UpdateAdminRole", admin.Id, $"Role changed to {role}");

            return Ok(new { message = "Admin role updated successfully." });
        }

        /// <summary>
        /// Deactivate an admin account.
        /// </summary>
        /// <param name="id">Admin database ID.</param>
        /// <remarks>
        /// Disables an admin account from accessing the system.
        /// The currently logged-in super admin cannot deactivate himself.
        /// </remarks>
        /// <response code="200">Admin deactivated successfully.</response>
        /// <response code="400">Invalid action, such as trying to deactivate yourself.</response>
        /// <response code="404">Admin not found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Super admin access is required.</response>
        [HttpPatch("{id}/deactivate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeactivateAdmin(int id)
        {
            var currentAdminId = GetCurrentAdminId();
            if (id == currentAdminId)
                return BadRequest(new { message = "You cannot deactivate yourself." });

            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound(new { message = "Admin not found." });

            admin.IsActive = false;
            await _adminRepo.SaveChangesAsync();
            await LogAction("DeactivateAdmin", admin.Id, "Admin deactivated");

            return Ok(new { message = "Admin deactivated successfully." });
        }

        /// <summary>
        /// Activate an admin account.
        /// </summary>
        /// <param name="id">Admin database ID.</param>
        /// <remarks>
        /// Re-enables a previously deactivated admin account.
        /// Only the super admin can perform this action.
        /// </remarks>
        /// <response code="200">Admin activated successfully.</response>
        /// <response code="404">Admin not found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden. Super admin access is required.</response>
        [HttpPatch("{id}/activate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ActivateAdmin(int id)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null) return NotFound(new { message = "Admin not found." });

            admin.IsActive = true;
            await _adminRepo.SaveChangesAsync();
            await LogAction("ActivateAdmin", admin.Id, "Admin activated");

            return Ok(new { message = "Admin activated successfully." });
        }
    }
}