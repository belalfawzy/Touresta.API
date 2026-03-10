using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAFIQ.API.DTOs.Admin;
using RAFIQ.API.DTOs.Common;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Controllers
{
    /// <summary>
    /// Admin audit log endpoints.
    /// </summary>
    [ApiController]
    [Route("api/admin/audit")]
    [Produces("application/json")]
    [Tags("Admin Audit")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminAuditController : ControllerBase
    {
        private readonly IAdminAuditLogRepository _auditRepo;

        public AdminAuditController(IAdminAuditLogRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }

        /// <summary>
        /// Get paged audit logs.
        /// </summary>
        /// <param name="query">Filtering and pagination options for audit records.</param>
        /// <response code="200">Audit logs retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AdminAuditQueryRequest query)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

            var logsQuery = _auditRepo.Query();

            if (!string.IsNullOrWhiteSpace(query.TargetType))
                logsQuery = logsQuery.Where(x => x.TargetType == query.TargetType);

            if (!string.IsNullOrWhiteSpace(query.TargetId))
                logsQuery = logsQuery.Where(x => x.TargetId == query.TargetId);

            if (!string.IsNullOrWhiteSpace(query.AdminId))
                logsQuery = logsQuery.Where(x => x.AdminId == query.AdminId);

            if (!string.IsNullOrWhiteSpace(query.Action))
                logsQuery = logsQuery.Where(x => x.Action == query.Action);

            var totalCount = logsQuery.Count();

            var items = logsQuery
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AdminAuditLogItemResponse
                {
                    Id = x.Id,
                    AdminId = x.AdminId,
                    Action = x.Action,
                    TargetType = x.TargetType,
                    TargetId = x.TargetId,
                    Details = x.Details,
                    Timestamp = x.Timestamp,
                    IpAddress = x.IpAddress
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var response = new PagedResponse<AdminAuditLogItemResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };

            return Ok(new
            {
                message = "Audit logs retrieved successfully.",
                data = response
            });
        }

        /// <summary>
        /// Get audit history for a specific helper.
        /// </summary>
        /// <param name="helperId">Helper database ID.</param>
        /// <response code="200">Helper audit logs retrieved successfully.</response>
        /// <response code="401">Unauthorized. Admin token is required.</response>
        [HttpGet("helpers/{helperId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetHelperAuditLogs(string helperId)
        {
            var logs = await _auditRepo.GetByTargetAsync("Helper", helperId);

            var data = logs.Select(x => new AdminAuditLogItemResponse
            {
                Id = x.Id,
                AdminId = x.AdminId,
                Action = x.Action,
                TargetType = x.TargetType,
                TargetId = x.TargetId,
                Details = x.Details,
                Timestamp = x.Timestamp,
                IpAddress = x.IpAddress
            }).ToList();

            return Ok(new
            {
                message = "Helper audit logs retrieved successfully.",
                count = data.Count,
                data
            });
        }
    }
}
