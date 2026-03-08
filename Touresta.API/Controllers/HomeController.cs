using Microsoft.AspNetCore.Mvc;
using Touresta.API.Data;

namespace Touresta.API.Controllers
{
    /// <summary>
    /// Health check and status endpoints for the Touresta API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Health Check")]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get API status.
        /// </summary>
        /// <remarks>
        /// Returns a basic health check confirming the API is running.
        ///
        /// **Example Response:**
        ///
        ///     {
        ///         "message": "Touresta API is running",
        ///         "version": "1.0",
        ///         "status": "Active"
        ///     }
        /// </remarks>
        /// <response code="200">API is running.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Touresta API is running",
                version = "1.0",
                status = "Active"
            });
        }

        /// <summary>
        /// Get current server status with timestamp.
        /// </summary>
        /// <remarks>
        /// Returns the current status and UTC timestamp.
        ///
        /// **Example Response:**
        ///
        ///     {
        ///         "status": "OK",
        ///         "timestamp": "2025-01-15T12:00:00Z"
        ///     }
        /// </remarks>
        /// <response code="200">Server status returned.</response>
        [HttpGet("status")]
        [ProducesResponseType(200)]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "OK", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Get application info.
        /// </summary>
        /// <remarks>
        /// Returns application name, version, and environment.
        ///
        /// **Example Response:**
        ///
        ///     {
        ///         "app": "Touresta API",
        ///         "version": "1.0",
        ///         "environment": "Production"
        ///     }
        /// </remarks>
        /// <response code="200">App info returned.</response>
        [HttpGet("info")]
        [ProducesResponseType(200)]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                app = "Touresta API",
                version = "1.0",
                environment = "Production"
            });
        }
        /// <summary>
        /// Deep health check verifying database connectivity.
        /// </summary>
        /// <response code="200">All systems healthy.</response>
        /// <response code="503">One or more systems unhealthy.</response>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> HealthCheck()
        {
            var dbHealthy = false;
            try
            {
                dbHealthy = await _db.Database.CanConnectAsync();
            }
            catch { }

            var status = dbHealthy ? "healthy" : "unhealthy";
            var result = new
            {
                status,
                checks = new { database = dbHealthy },
                timestamp = DateTime.UtcNow
            };

            return dbHealthy ? Ok(result) : StatusCode(503, result);
        }
    }
}
